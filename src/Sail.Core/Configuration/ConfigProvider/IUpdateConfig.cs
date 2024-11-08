using Yarp.ReverseProxy.Configuration;

namespace Sail.Core.Configuration.ConfigProvider;

public interface IUpdateConfig
{
    Task UpdateAsync(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters,
        CancellationToken cancellationToken);
}