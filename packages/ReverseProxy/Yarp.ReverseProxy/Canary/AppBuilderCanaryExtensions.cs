using Yarp.ReverseProxy.Canary;

namespace  Microsoft.AspNetCore.Builder;

public static class AppBuilderCanaryExtensions
{
    public static IReverseProxyApplicationBuilder UseCanary(this IReverseProxyApplicationBuilder builder)
    {
        builder.UseMiddleware<CanaryMiddleware>();
        return builder;
    }
}