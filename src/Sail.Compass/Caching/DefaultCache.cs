using Sail.Api.V1;
using Yarp.ReverseProxy.Configuration;

namespace Sail.Compass.Caching;

public class DefaultCache : ICache
{
    private readonly Dictionary<string, RouteConfig> routeConfigs = new();
    
    public ValueTask UpdateAsync(IReadOnlyList<Route> routes, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}