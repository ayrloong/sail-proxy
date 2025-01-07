using System.Collections.Immutable;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sail.Api.V1;
using Sail.Compass.Caching;
using Sail.Compass.Hosting;
using Sail.Compass.Informers;
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
        IResourceInformer<Route> routeInformer,
        IResourceInformer<Cluster> clusterInformer,
        ILogger<ProxyDiscoveryService> logger) : base(hostApplicationLifetime, logger)
    {
        var registrations = new List<IResourceInformerRegistration>()
        {
            routeInformer.Register(Notification),
            clusterInformer.Register(Notification),
        };

        _cache = cache;
        _reconciler = reconciler;
        _registrations = registrations;
        _registrationsReady = false;
        
        routeInformer.StartWatching();
        clusterInformer.StartWatching();
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
                Console.WriteLine("Proxy discovery");
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

    private void Notification(ResourceEvent<Route> resource)
    {
        NotificationChanged();
    }

    private void Notification(ResourceEvent<Cluster> resource)
    {
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