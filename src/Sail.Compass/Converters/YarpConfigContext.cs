using Yarp.ReverseProxy.Configuration;

namespace Sail.Compass.Converters;

internal class YarpConfigContext
{
    public List<ClusterConfig> Clusters { get; } = [];
    public List<RouteConfig> Routes { get; } = [];
}