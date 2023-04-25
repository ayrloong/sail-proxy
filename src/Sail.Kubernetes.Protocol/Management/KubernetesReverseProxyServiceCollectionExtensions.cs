using Microsoft.Extensions.DependencyInjection.Extensions;
using Sail.Kubernetes.Protocol.Plugins;

namespace Microsoft.Extensions.DependencyInjection;

public static class KubernetesReverseProxyServiceCollectionExtensions
{
    public static IServiceCollection AddKubernetesPlugin(this IServiceCollection services)
    {
        services.AddSingleton<IPluginUpdater, PluginUpdater>();
        services.TryAddEnumerable(new[]
        {
            ServiceDescriptor.Singleton<IPlugin, CorsPlugin>(),
            ServiceDescriptor.Singleton<IPlugin, JwtBearerPlugin>(),
        });
        return services;
    }
}