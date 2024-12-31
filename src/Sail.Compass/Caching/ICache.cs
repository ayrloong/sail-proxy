using Sail.Api.V1;

namespace Sail.Compass.Caching;

public interface ICache
{
    void UpdateRoutes(IReadOnlyList<Route> routes);
    void UpdateClusters(IReadOnlyList<Cluster> clusters);
    List<Route> GetRoutes();
    List<Cluster> GetClusters();
}