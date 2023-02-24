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

    public Task InvokeAsync(HttpContext context)
    {
        var proxyFeature = context.GetReverseProxyFeature();
        var clusters = proxyFeature.Route.Config.Clusters;
        if (clusters is not null && clusters.Any())
        {

        }

        return _next(context);
    }
}