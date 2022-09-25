using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Yarp.ReverseProxy.Canary;

public class CanaryMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IProxyStateLookup _lookup;

    public CanaryMiddleware(RequestDelegate next, IProxyStateLookup lookup)
    {
        _next = next;
        _lookup = lookup;
    }

    public Task Invoke(HttpContext context)
    {
        var proxyFeature = context.GetReverseProxyFeature();
        var weightCluster = proxyFeature.Route.Config.WeightCluster;
        if (weightCluster is not null)
        {
            var dictionary = weightCluster?.Clusters.ToDictionary(x => x.ClusterId, x => (float)x.Weight / 100);
            var selectedCluster = dictionary.SelectedWeight(x => x.Value);
            if (_lookup.TryGetCluster(selectedCluster.Key, out var cluster))
            {
                context.ReassignProxyRequest(cluster);
            }
        }
        
        return _next(context);
    }
}