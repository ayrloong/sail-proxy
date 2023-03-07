using Microsoft.Extensions.DependencyInjection.Extensions;
using Sail.Kubernetes.Protocol.Middlewares;

namespace Microsoft.Extensions.DependencyInjection;

public static class KubernetesReverseProxyServiceCollectionExtensions
{
    public static IServiceCollection AddKubernetesMiddleware(this IServiceCollection services)
    {
        services.AddSingleton<IMiddlewareUpdater, MiddlewareUpdater>();
        services.TryAddEnumerable(new[]
        {
            ServiceDescriptor.Singleton<IMiddleware, CorsMiddleware>(),
            ServiceDescriptor.Singleton<IMiddleware, JwtBearerMiddleware>(),
        });
        return services;
    }
}