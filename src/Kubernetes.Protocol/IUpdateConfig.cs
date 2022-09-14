using Yarp.ReverseProxy.Configuration;

namespace Sail.Kubernetes.Protocol;
public interface IUpdateConfig
{
    void Update(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters);
}
