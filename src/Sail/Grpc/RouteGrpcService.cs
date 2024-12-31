using Grpc.Core;
using Sail.Api.V1;
using Sail.Core.Stores;
using Route = Sail.Core.Entities.Route;
using RouteResponse = Sail.Api.V1.Route;
using RouteHeaderResponse = Sail.Api.V1.RouteMatch.Types.RouteHeader;
using QueryParametersResponse = Sail.Api.V1.RouteMatch.Types.RouteQueryParameter;

namespace Sail.Grpc;

public class RouteGrpcService(IRouteStore routeStore) : RouteService.RouteServiceBase
{
    public override async Task StreamRoutes(RouteRequest request, IServerStreamWriter<RouteItems> responseStream,
        ServerCallContext context)
    {
        while (!context.CancellationToken.IsCancellationRequested)
        {
            var clusters = await routeStore.GetAsync(CancellationToken.None);
            var response = MapToDiscoveryResponse(clusters);

            await responseStream.WriteAsync(response);
            await Task.Delay(TimeSpan.FromSeconds(20));
        }
    }

    private static RouteItems MapToDiscoveryResponse(List<Route> routes)
    {

        var items = routes.Select(MapToRouteResponse);

        var response = new RouteItems
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
            ClusterId = route.ClusterId.ToString(),
            Match = new RouteMatch
            {
                Path = match.Path,
                Hosts = { match.Hosts },
                Methods = { match.Methods },
                Headers =
                {
                    match.Headers.Select(header => new RouteHeaderResponse
                    {
                        Name = header.Name,
                        Values = { header.Values },
                        IsCaseSensitive = header.IsCaseSensitive,
                    })
                },
                QueryParameters =
                {
                    match.QueryParameters.Select(parameter => new QueryParametersResponse
                    {
                        Name = parameter.Name,
                        Values = { parameter.Values },
                        IsCaseSensitive = parameter.IsCaseSensitive,
                    })
                }
            },
            /**
             Order = route.Order,
             CorsPolicy = route.CorsPolicy,
             TimeoutPolicy = route.TimeoutPolicy,
             AuthorizationPolicy = route.AuthorizationPolicy,
             MaxRequestBodySize = route.MaxRequestBodySize ?? -1,
             RateLimiterPolicy = route.RateLimiterPolicy,
             **/
        };
    }
}