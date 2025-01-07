using System.Reactive.Linq;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sail.Api.V1;

namespace Sail.Compass.Informers;

public class V1RouteResourceInformer(
    IHostApplicationLifetime hostApplicationLifetime,
    RouteService.RouteServiceClient client,
    ILogger<V1RouteResourceInformer> logger) : ResourceInformer<Route>(hostApplicationLifetime, logger)
{
    protected override IObservable<ResourceEvent<Route>> GetObservable(bool watch)
    {
        var result = watch ? Watch() : List();
        return result;
    }

    private IObservable<ResourceEvent<Route>> List()
    {
        return Observable.Create<ResourceEvent<Route>>(async (observer, cancellationToken) =>
        {
            var list = client.List(new Empty(), cancellationToken: cancellationToken);
            foreach (var item in list.Items)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;
                var resource = item.ToResourceEvent(EventType.List);
                observer.OnNext(resource!);
            }

            observer.OnCompleted();
        });
    }

    private IObservable<ResourceEvent<Route>> Watch()
    {
        return Observable.Create<ResourceEvent<Route>>(async (observer, cancellationToken) =>
        {
            var result = client.Watch(new Empty(), cancellationToken: cancellationToken);
            var watch = result.ResponseStream;
            await foreach (var current in watch.ReadAllAsync(cancellationToken: cancellationToken))
            {
                var resource = current.Route.ToResourceEvent(EventType.Created);
                observer.OnNext(resource!);
            }
        });
    }
}