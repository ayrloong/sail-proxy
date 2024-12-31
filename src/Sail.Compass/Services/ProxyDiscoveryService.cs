using System.Collections.Immutable;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sail.Api.V1;
using Sail.Compass.Caching;
using Sail.Compass.Client;
using Sail.Compass.Hosting;
using Sail.Compass.Queues;

namespace Sail.Compass.Services;

public class ProxyDiscoveryService : BackgroundHostedService
{
    private bool _registrationsReady;
    private readonly ICache _cache;
    private readonly IReconciler _reconciler;
    private readonly IWorkQueue<QueueItem> _queue;
    private readonly IReadOnlyList<IResourceInformerRegistration> _registrations;
    private readonly QueueItem _changeQueueItem;

    public ProxyDiscoveryService(
        ICache cache,
        IReconciler reconciler,
        IHostApplicationLifetime hostApplicationLifetime,
        IResourceInformer<ClusterItems> clusterInformer,
        IResourceInformer<RouteItems> routeInformer,
        ILogger<ProxyDiscoveryService> logger) : base(hostApplicationLifetime, logger)
    {
        var registrations = new List<IResourceInformerRegistration>()
        {
            clusterInformer.Register(Notification),
            routeInformer.Register(Notification),
        };

        _cache = cache;
        _reconciler = reconciler;
        _registrations = registrations;
        _registrationsReady = false;
        
        clusterInformer.StartWatching();
        routeInformer.StartWatching();
        _queue = new ProcessingRateLimitedQueue<QueueItem>(perSecond: 0.5, burst: 1);
        _changeQueueItem = new QueueItem("");
    }

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        foreach (var registration in _registrations)
        {
            await registration.ReadyAsync(cancellationToken).ConfigureAwait(false);
        }

        _registrationsReady = true;
        NotificationChanged();

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
            }
            catch
            {
                Logger.LogInformation("Rescheduling {Change}", item.Change);
                _queue.Add(item);
            }
            finally
            {
                _queue.Done(item);
            }
        }
    }

    private void Notification(RouteItems resource)
    {
         _cache.UpdateRoutes(resource.Items);
        
        NotificationChanged();
    }

    private void Notification(ClusterItems resource)
    {
        _cache.UpdateClusters(resource.Items);
        
        NotificationChanged();
    }

    private void NotificationChanged()
    {
        if (!_registrationsReady)
        {
            return;
        }

        _queue.Add(_changeQueueItem);
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