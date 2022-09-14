using k8s;
using k8s.Models;
using Microsoft.Kubernetes;
using Microsoft.Kubernetes.Controller.Hosting;
using Microsoft.Kubernetes.Controller.Informers;
using Microsoft.Kubernetes.Controller.Queues;
using Sail.Kubernetes.Gateway.Caching;
using Sail.Kubernetes.Gateway.Dispatching;
using Sail.Kubernetes.Gateway.Models;

namespace Sail.Kubernetes.Gateway.Services;

public class GatewayController : BackgroundHostedService
{

    private readonly IResourceInformerRegistration[] _registrations;
    private readonly ICache _cache;
    private readonly IReconciler _reconciler;

    private bool _registrationsReady;
    private readonly IWorkQueue<QueueItem> _queue;
    private readonly QueueItem _ingressChangeQueueItem;

    public GatewayController(
        ICache cache,
        IReconciler reconciler,
        IResourceInformer<V1beta1Gateway> gatewayInformer,
        IResourceInformer<V1Service> serviceInformer,
        IResourceInformer<V1Endpoints> endpointsInformer,
        IResourceInformer<V1beta1GatewayClass> gatewayClassInformer,
        IResourceInformer<V1beta1HttpRoute> httpRouteInformer,
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger<GatewayController> logger)
        : base(hostApplicationLifetime, logger)
    {
        if (gatewayInformer is null)
        {
            throw new ArgumentNullException(nameof(gatewayInformer));
        }

        if (serviceInformer is null)
        {
            throw new ArgumentNullException(nameof(serviceInformer));
        }

        if (endpointsInformer is null)
        {
            throw new ArgumentNullException(nameof(endpointsInformer));
        }

        if (gatewayClassInformer is null)
        {
            throw new ArgumentNullException(nameof(gatewayClassInformer));
        }

        if (httpRouteInformer is null)
        {
            throw new ArgumentNullException(nameof(httpRouteInformer));
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
            serviceInformer.Register(Notification),
            endpointsInformer.Register(Notification),
            gatewayClassInformer.Register(Notification),
            gatewayInformer.Register(Notification),
            httpRouteInformer.Register(Notification)
        };
        _registrationsReady = false;
        serviceInformer.StartWatching();
        endpointsInformer.StartWatching();
        gatewayClassInformer.StartWatching();
        gatewayInformer.StartWatching();
        httpRouteInformer.StartWatching();

        _queue = new ProcessingRateLimitedQueue<QueueItem>(perSecond: 0.5, burst: 1);

        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _reconciler = reconciler ?? throw new ArgumentNullException(nameof(reconciler));
        _reconciler.OnAttach(TargetAttached);

        _ingressChangeQueueItem = new QueueItem("Gateway Change", null);
    }


    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        foreach (var registration in _registrations)
        {
            await registration.ReadyAsync(cancellationToken).ConfigureAwait(false);
        }

        _registrationsReady = true;
        NotificationIngressChanged();
        while (!cancellationToken.IsCancellationRequested)
        {
            var (item, shutdown) = await _queue.GetAsync(cancellationToken).ConfigureAwait(false);
            if (shutdown)
            {
                return;
            }
            
            try
            {
                await _reconciler.ProcessAsync(cancellationToken).ConfigureAwait(false);
                _queue.Done(item);
            }
            catch
            {
                Logger.LogInformation("Rescheduling {Change}", item.Change);
                _queue.Add(item);
            }
        }
    }


    private void Notification(WatchEventType eventType, V1beta1Gateway resource)
    {
        if (_cache.Update(eventType, resource))
        {
            NotificationIngressChanged();
        }
    }

    private void Notification(WatchEventType eventType, V1beta1HttpRoute resource)
    {
        if (_cache.Update(eventType, resource))
        {
            NotificationIngressChanged();
        }
    }

    private void NotificationIngressChanged()
    {
        if (!_registrationsReady)
        {
            return;
        }

        _queue.Add(_ingressChangeQueueItem);
    }

    private void Notification(WatchEventType eventType, V1Service resource)
    {
        var gatewayNames = _cache.Update(eventType, resource);
        if (gatewayNames.Count > 0)
        {
            NotificationIngressChanged();
        }
    }

    private void Notification(WatchEventType eventType, V1Endpoints resource)
    {
        var gatewayNames = _cache.Update(eventType, resource);
        if (gatewayNames.Count > 0)
        {
            NotificationIngressChanged();
        }
    }

    private void Notification(WatchEventType eventType, V1beta1GatewayClass resource)
    {
        _cache.Update(eventType, resource);
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
}