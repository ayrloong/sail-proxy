using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Yarp.ReverseProxy.Canary;

internal sealed class CanaryMiddleware
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
        var canary = proxyFeature.Cluster.Config.Canary;
        
        if (!canary.Enabled ?? false)
        {
            return _next(context);
        }
        
        var customHeader = canary.CustomHeader;
        var customHeaderValue = canary.CustomHeaderValue;
        if (!string.IsNullOrEmpty(customHeader))
        {
            var headerValue = context.Request.Headers[customHeader];
            if (headerValue == customHeaderValue)
            {
                return _next(context);
            }
        }

        var clusters = _lookup.GetClusters();

        var currentCluster = clusters.FirstOrDefault();
        if (_lookup.TryGetCluster(currentCluster.ClusterId, out var cluster))
        {
            context.ReassignProxyRequest(cluster);
        }
        
        return _next(context);
    }
}