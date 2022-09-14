using Microsoft.AspNetCore.Http;
using Sail.Model;
using Yarp.ReverseProxy;

namespace Sail.Canary;

public class CanaryMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IProxyStateLookup _lookup;

    public Task Invoke(HttpContext context)
    {
        var proxyFeature = context.GetProxyFeature();
        return _next(context);
    }
}