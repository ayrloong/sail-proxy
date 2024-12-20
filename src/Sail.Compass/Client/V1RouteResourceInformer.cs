using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sail.Api.V1;

namespace Sail.Compass.Client;

public class V1RouteResourceInformer(
    IHostApplicationLifetime hostApplicationLifetime,
    RouteService.RouteServiceClient client,
    ILogger<V1RouteResourceInformer> logger) : ResourceInformer<RouteItems>(hostApplicationLifetime, logger)
{

    protected override Task<IAsyncStreamReader<RouteItems>> RetrieveResourceAsync(
        CancellationToken cancellationToken = default)
    {
        var response = client.StreamRoutes(new RouteRequest());
        return Task.FromResult(response.ResponseStream);
    }
}