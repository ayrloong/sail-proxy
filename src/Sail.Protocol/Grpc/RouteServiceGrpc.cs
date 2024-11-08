using Grpc.Core;

namespace Sail.Protocol.Grpc;

public class RouteServiceGrpc : RouteService.RouteServiceBase
{
    public override Task<CreateRouteResponse> Create(Route request, ServerCallContext context)
    {
        throw new NotImplementedException();
    }
}