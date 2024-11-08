using Sail.Core.Entities;

namespace Sail.Core.Converters;

internal class DataSourceContext(List<Cluster> clusters, List<Route> routes)
{
    public List<Cluster> Clusters { get; } = clusters;
    public List<Route> Routes { get; } = routes;
}