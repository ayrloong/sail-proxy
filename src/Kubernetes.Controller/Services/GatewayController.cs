using k8s;
using k8s.Models;
using Microsoft.Kubernetes;
using Microsoft.Kubernetes.Controller.Hosting;
using Microsoft.Kubernetes.Controller.Informers;
using Microsoft.Kubernetes.Controller.Queues;
using Sail.Kubernetes.Controller.Caching;
using Sail.Kubernetes.Controller.Dispatching;
using Sail.Kubernetes.Controller.Models;

namespace Sail.Kubernetes.Controller.Services;

public class GatewayController : BackgroundHostedService
{
    private readonly IResourceInformerRegistration[] _registrations;
    private readonly ICache _cache;
    private readonly IReconciler _reconciler;

    private bool _registrationsReady;
    private readonly IWorkQueue<QueueItem> _queue;
    private readonly QueueItem _gatewayChangeQueueItem;

    public GatewayController(
        ICache cache,
        IReconciler reconciler,
        IResourceInformer<V1beta1Gateway> gatewayInformer,
        IResourceInformer<V1beta1GatewayClass> gatewayClassInformer,
        IResourceInformer<V1beta1HttpRoute> httpRouteInformer,
        IResourceInformer<V1Endpoints> endpointsInformer,
        IResourceInformer<V1Service> serviceInformer,
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger<GatewayController> logger)
        : base(hostApplicationLifetime, logger)
    {
        if (gatewayInformer is null)
        {
            throw new ArgumentNullException(nameof(gatewayInformer));
        }

        if (gatewayClassInformer is null)
        {
            throw new ArgumentNullException(nameof(gatewayClassInformer));
        }

        if (httpRouteInformer is null)
        {
            throw new ArgumentNullException(nameof(httpRouteInformer));
        }

        if (serviceInformer is null)
        {
            throw new ArgumentNullException(nameof(serviceInformer));
        }

        if (hostApplicationLifetime is null)
        {
            throw new ArgumentNullException(nameof(hostApplicationLifetime));
        }

        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        _registrations = new[]
        {
            gatewayInformer.Register(Notification),
            gatewayClassInformer.Register(Notification),
            httpRouteInformer.Register(Notification),
            serviceInformer.Register(Notification),
            endpointsInformer.Register(Notification),
        };

        _registrationsReady = false;
        gatewayInformer.StartWatching();
        gatewayClassInformer.StartWatching();
        httpRouteInformer.StartWatching();
        serviceInformer.StartWatching();
        endpointsInformer.StartWatching();

        _queue = new ProcessingRateLimitedQueue<QueueItem>(perSecond: 0.5, burst: 1);
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _reconciler = reconciler ?? throw new ArgumentNullException(nameof(reconciler));
        _reconciler.OnAttach(TargetAttached);

        _gatewayChangeQueueItem = new QueueItem("Gateway Change", null);
    }

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        foreach (var registration in _registrations)
        {
            await registration.ReadyAsync(cancellationToken).ConfigureAwait(false);
        }

        // At this point we know that all of the Ingress and Endpoint caches are at least in sync
        // with cluster's state as of the start of this controller.
        _registrationsReady = true;
        NotificationGatewayChanged();
        // Now begin one loop to process work until an application shutdown is requested.
        while (!cancellationToken.IsCancellationRequested)
        {
            // Dequeue the next item to process
            var (item, shutdown) = await _queue.GetAsync(cancellationToken).ConfigureAwait(false);
            if (shutdown)
            {
                Logger.LogInformation("Work queue has been shutdown. Exiting reconciliation loop.");
                return;
            }

            try
            {
                await _reconciler.ProcessAsync(cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                Logger.LogInformation("Rescheduling {Change}", item.Change);
                // Any failure to process this item results in being re-queued
                _queue.Add(item);
            }
            finally
            {
                _queue.Done(item);
            }
        }

        Logger.LogInformation("Reconciliation loop cancelled");
    }

    private void NotificationGatewayChanged()
    {
        if (!_registrationsReady)
        {
            return;
        }

        _queue.Add(_gatewayChangeQueueItem);
    }

    private void TargetAttached(IDispatchTarget target)
    {
        var keys = new List<NamespacedName>();
        _cache.GetKeys(keys);
        if (keys.Count > 0)
        {
            _queue.Add(new QueueItem("Target Attached", target));
        }
    }

    private void Notification(WatchEventType eventType, V1beta1Gateway resource)
    {
        if (_cache.Update(eventType, resource))
        {
            NotificationGatewayChanged();
        }
    }

    private void Notification(WatchEventType eventType, V1beta1GatewayClass resource)
    {
        _cache.Update(eventType, resource);
    }

    private void Notification(WatchEventType eventType, V1beta1HttpRoute resource)
    {
        var gatewayNames = _cache.Update(eventType, resource);
        if (gatewayNames.Count > 0)
        {
            NotificationGatewayChanged();
        }
    }

    private void Notification(WatchEventType eventType, V1Service resource)
    {
        var gatewayNames = _cache.Update(eventType, resource);
        if (gatewayNames.Count > 0)
        {
            NotificationGatewayChanged();
        }
    }
    
    private void Notification(WatchEventType eventType, V1Endpoints resource)
    {
        var gatewayNames = _cache.Update(eventType, resource);
        if (gatewayNames.Count > 0)
        {
            NotificationGatewayChanged();
        }
    }
    
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (var registration in _registrations)
            {
                registration.Dispose();
            }

            _queue.Dispose();
        }

        base.Dispose(disposing);
    }
}