using Microsoft.AspNetCore.Builder;

namespace Yarp.ReverseProxy.Canary;

internal static class AppBuilderCanaryExtensions
{
    public static IReverseProxyApplicationBuilder UseCanary(this IReverseProxyApplicationBuilder builder)
    {
        builder.UseMiddleware<CanaryMiddleware>();
        return builder;
    }
}