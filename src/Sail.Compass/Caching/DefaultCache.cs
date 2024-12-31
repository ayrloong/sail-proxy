using Sail.Api.V1;


namespace Sail.Compass.Caching;

public class DefaultCache : ICache
{
    private readonly Dictionary<string, Route> _routes = new();
    private readonly Dictionary<string, Cluster> _clusters = new();

    public  void UpdateRoutes(IReadOnlyList<Route> routes)
    {
        foreach (var route in routes)
        {
            _routes[route.RouteId] = route;
        }
    }

    public void UpdateClusters(IReadOnlyList<Cluster> clusters)
    {
        foreach (var cluster in clusters)
        {
            _clusters[cluster.ClusterId] = cluster;
        }
    }

    public List<Route> GetRoutes()
    {
        return _routes.Values.ToList();
    }

    public List<Cluster> GetClusters()
    {
        return _clusters.Values.ToList();
    }
}