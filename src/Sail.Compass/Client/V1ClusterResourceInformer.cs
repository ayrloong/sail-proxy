using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sail.Api.V1;

namespace Sail.Compass.Client;

public class V1ClusterResourceInformer(
    IHostApplicationLifetime hostApplicationLifetime,
    ClusterService.ClusterServiceClient client,
    ILogger<V1ClusterResourceInformer> logger) : ResourceInformer<ClusterItems>(hostApplicationLifetime, logger)
{
    protected override Task<IAsyncStreamReader<ClusterItems>> RetrieveResourceAsync(
        CancellationToken cancellationToken = default)
    {
        var response = client.StreamClusters(new ClusterRequest());
        return Task.FromResult(response.ResponseStream);
    }
}