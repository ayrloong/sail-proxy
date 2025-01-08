using Sail.Api.V1;
using Sail.Compass.Informers;
using EventType = Sail.Compass.Informers.EventType;


namespace Sail.Compass.Caching;

public class DefaultCache : ICache
{
    private readonly Dictionary<string, Route> _routes = new();
    private readonly Dictionary<string, Cluster> _clusters = new();

    public void UpdateRoute(ResourceEvent<Route> resource)
    {
        var id = resource.Value.RouteId;
        if (resource.EventType == EventType.Deleted)
        {
            _routes.Remove(id);
            return;
        }

        _routes[id] = resource.Value;
    }

    public void UpdateCluster(ResourceEvent<Cluster> resource)
    {
        var id = resource.Value.ClusterId;
        if (resource.EventType == EventType.Deleted)
        {
            _clusters.Remove(id);
            return;
        }

        _clusters[id] = resource.Value;
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