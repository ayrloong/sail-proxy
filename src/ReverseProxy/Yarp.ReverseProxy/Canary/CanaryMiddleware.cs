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
        return _next(context);
    }
}