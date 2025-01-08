using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MongoDB.Driver;
using Sail.Api.V1;
using Sail.Core.Stores;
using Sail.Storage.MongoDB;
using Sail.Storage.MongoDB.Extensions;
using Route = Sail.Core.Entities.Route;
using RouteResponse = Sail.Api.V1.Route;

namespace Sail.Grpc;

public class RouteGrpcService(SailContext dbContext,IRouteStore routeStore) : RouteService.RouteServiceBase
{
    public override async Task<ListRouteResponse> List(Empty request, ServerCallContext context)
    {
        var clusters = await routeStore.GetAsync(CancellationToken.None);
        var response = MapToRouteItemsResponse(clusters);
        return response;
    }
    
    public override async Task Watch(Empty request, IServerStreamWriter<WatchRouteResponse> responseStream,
        ServerCallContext context)
    {
        var options = new ChangeStreamOptions
        {
            FullDocument = ChangeStreamFullDocumentOption.Default,
            FullDocumentBeforeChange = ChangeStreamFullDocumentBeforeChangeOption.Required
        };

        while (!context.CancellationToken.IsCancellationRequested)
        {
            var watch = await dbContext.Routes.WatchAsync(options);

            await foreach (var changeStreamDocument in watch.ToAsyncEnumerable())
            {
                var document = changeStreamDocument.FullDocument;
                
                if (changeStreamDocument.OperationType == ChangeStreamOperationType.Delete)
                {
                    document = changeStreamDocument.FullDocumentBeforeChange;
                }

                var eventType = changeStreamDocument.OperationType switch
                {
                    ChangeStreamOperationType.Create => EventType.Create,
                    ChangeStreamOperationType.Update => EventType.Update,
                    ChangeStreamOperationType.Delete => EventType.Delete,
                    _ => EventType.Unknown
                };
                var route = MapToRouteResponse(document);

                var response = new WatchRouteResponse
                {
                    Route =  route,
                    EventType = eventType
                };
                await responseStream.WriteAsync(response);
            }
        }
    }

    private static ListRouteResponse MapToRouteItemsResponse(List<Route> routes)
    {
        var items = routes.Select(MapToRouteResponse);

        var response = new ListRouteResponse
        {
            Items = { items }
        };
        return response;
    }

    private static RouteResponse MapToRouteResponse(Route route)
    {
        var match = route.Match;
        return new RouteResponse
        {
            RouteId = route.Id.ToString(),
            Match = new RouteMatch(),
            Order = route.Order,
            CorsPolicy = route.CorsPolicy,
            TimeoutPolicy = route.TimeoutPolicy,
            AuthorizationPolicy = route.AuthorizationPolicy,
            MaxRequestBodySize = route.MaxRequestBodySize ?? -1,
            RateLimiterPolicy = route.RateLimiterPolicy
        };
    }
}