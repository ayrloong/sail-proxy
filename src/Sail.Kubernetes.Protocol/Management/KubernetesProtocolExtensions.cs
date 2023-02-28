using Sail.Kubernetes.Protocol.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class KubernetesProtocolExtensions
{
    public static IServiceCollection AddUpdater(this IServiceCollection services)
    {
        services.AddAuthentication();
        services.AddSingleton<IAuthenticationSchemeUpdater, AuthenticationSchemeUpdater>();
        services.AddSingleton<ICorsOptionsUpdater, CorsOptionsUpdater>();
        services.AddSingleton<IRateLimiterOptionsUpdater, RateLimiterOptionsUpdater>();
        services.AddSingleton<IMiddlewareUpdater, MiddlewareUpdater>();
        return services;
    }
}