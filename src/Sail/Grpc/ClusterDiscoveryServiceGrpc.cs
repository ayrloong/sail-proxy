using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Sail.Core.Entities;
using Sail.Services;

namespace Sail.Grpc;

public class ClusterDiscoveryServiceGrpc(IClusterService clusterService)
    : ClusterDiscoveryService.ClusterDiscoveryServiceBase
{
    public override async Task StreamClusters(IAsyncStreamReader<DiscoveryRequest> requestStream,
        IServerStreamWriter<DiscoveryResponse> responseStream, ServerCallContext context)
    {
        var clusters = await clusterService.GetAsync();
        
        while (await requestStream.MoveNext())
        {
            var version = requestStream.Current.VersionInfo;

            
            await Task.Delay(5000); 
        }
    }
}