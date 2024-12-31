using Sail.Api.V1;

namespace Sail.Compass.Converters;

internal class DataSourceContext(List<Cluster> clusters, List<Route> routes)
{
    public List<Cluster> Clusters { get; } = clusters;
    public List<Route> Routes { get; } = routes;
}