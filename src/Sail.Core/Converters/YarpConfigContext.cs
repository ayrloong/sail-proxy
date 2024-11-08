using Yarp.ReverseProxy.Configuration;

namespace Sail.Core.Converters;

internal class YarpConfigContext
{
    public List<ClusterConfig> Clusters { get; } = [];
    public List<RouteConfig> Routes { get; } = [];
}