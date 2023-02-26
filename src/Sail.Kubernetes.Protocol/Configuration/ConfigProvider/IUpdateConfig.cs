using Yarp.ReverseProxy.Configuration;

namespace Sail.Kubernetes.Protocol.Configuration;

public interface IUpdateConfig
{
    Task UpdateAsync(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters,IReadOnlyList<MiddlewareConfig> middlewares);
}