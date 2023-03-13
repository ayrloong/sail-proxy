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

public class IngressController : BackgroundHostedService
{
    private readonly IResourceInformerRegistration[] _registrations;
    private readonly ICache _cache;
    private readonly IReconciler _reconciler;

    private bool _registrationsReady;
    private readonly IWorkQueue<QueueItem> _queue;
    private readonly QueueItem _ingressChangeQueueItem;


    public IngressController(
        ICache cache,
        IReconciler reconciler,
        IResourceInformer<V1Ingress> ingressInformer,
        IResourceInformer<V1Service> serviceInformer,
        IResourceInformer<V1Endpoints> endpointsInformer,
        IResourceInformer<V1IngressClass> ingressClassInformer,
        IResourceInformer<V1beta1Middleware> middlewareInformer,
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger<IngressController> logger)
        : base(hostApplicationLifetime, logger)
    {
        if (ingressInformer is null)
        {
            throw new ArgumentNullException(nameof(ingressInformer));
        }

        if (serviceInformer is null)
        {
            throw new ArgumentNullException(nameof(serviceInformer));
        }

        if (endpointsInformer is null)
        {
            throw new ArgumentNullException(nameof(endpointsInformer));
        }

        if (ingressClassInformer is null)
        {
            throw new ArgumentNullException(nameof(ingressClassInformer));
        }

        if (middlewareInformer is null)
        {
            throw new ArgumentNullException(nameof(middlewareInformer));
        }

        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        _registrations = new[]
        {
            serviceInformer.Register(Notification),
            endpointsInformer.Register(Notification),
            ingressClassInformer.Register(Notification),
            ingressInformer.Register(Notification),
            middlewareInformer.Register(Notification)
        };
        _registrationsReady = false;
        serviceInformer.StartWatching();
        endpointsInformer.StartWatching();
        ingressClassInformer.StartWatching();
        ingressInformer.StartWatching();
        middlewareInformer.StartWatching();

        _queue = new ProcessingRateLimitedQueue<QueueItem>(perSecond: 0.5, burst: 1);

        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _reconciler = reconciler ?? throw new ArgumentNullException(nameof(reconciler));
        _reconciler.OnAttach(TargetAttached);

        _ingressChangeQueueItem = new QueueItem("ingress Change", null);

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

    private void TargetAttached(IDispatchTarget target)
    {
        var keys = new List<NamespacedName>();
        _cache.GetKeys(keys);
        if (keys.Any())
        {
            _queue.Add(new QueueItem("Target Attached", target));
        }
    }

    private void Notification(WatchEventType eventType,V1beta1Middleware resource)
    {
        if (_cache.Update(eventType, resource))
        {
            NotificationIngressChanged();
        }
    }
    private void Notification(WatchEventType eventType, V1IngressClass resource)
    {
        _cache.Update(eventType, resource);
    }

    private void Notification(WatchEventType eventType, V1Endpoints resource)
    {
        var ingressNames = _cache.Update(eventType, resource);
        if (ingressNames.Any())
        {
            NotificationIngressChanged();
        }
    }

    private void Notification(WatchEventType eventType, V1Service resource)
    {
        var ingressNames = _cache.Update(eventType, resource);
        if (ingressNames.Any())
        {
            NotificationIngressChanged();
        }
    }

    private void Notification(WatchEventType eventType, V1Ingress resource)
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