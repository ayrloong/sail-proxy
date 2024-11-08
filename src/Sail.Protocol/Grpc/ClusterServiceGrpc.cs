using Grpc.Core;

namespace Sail.Protocol.Grpc;

public class ClusterServiceGrpc : ClusterService.ClusterServiceBase
{
    public override Task<CreateClusterResponse> Create(Cluster request, ServerCallContext context)
    {
        throw new NotImplementedException();
    }
}