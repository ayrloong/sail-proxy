using Grpc.Core;
using Sail.Api.V1;
using Sail.Core.Stores;
using Google.Protobuf.WellKnownTypes;
using Cluster = Sail.Core.Entities.Cluster;
using ClusterResponse =  Sail.Api.V1.Cluster;

namespace Sail.Grpc;

public class ClusterGrpcService(IClusterStore clusterStore)
    : ClusterService.ClusterServiceBase
{
    public override async Task StreamClusters(ClusterRequest request,
        IServerStreamWriter<ClusterItems> responseStream, ServerCallContext context)
    {

        while (!context.CancellationToken.IsCancellationRequested)
        {
            var clusters = await clusterStore.GetAsync(CancellationToken.None);
            var response = MapToDiscoveryResponse(clusters);
        
            await responseStream.WriteAsync(response);
            await Task.Delay(TimeSpan.FromSeconds(20));
        }
    }

    private static ClusterItems MapToDiscoveryResponse(List<Cluster> clusters)
    {
        var items = clusters.Select(MapToClusterResponse);
        
        var response = new ClusterItems
        {
            Items = { items }
        };
        return response;
    }

    private static ClusterResponse MapToClusterResponse(Cluster cluster)
    {

        return new ClusterResponse
        {
            ClusterId = cluster.Id.ToString(),
            LoadBalancingPolicy = cluster.LoadBalancingPolicy
        };
    }
}