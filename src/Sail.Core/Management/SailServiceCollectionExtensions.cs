using Microsoft.Extensions.DependencyInjection.Extensions;
using Sail.Core.Certificates;
using Sail.Core.Configuration.ConfigProvider;
using Sail.Core.Hosting;
using Sail.Core.Options;
using Yarp.ReverseProxy.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class SailServiceCollectionExtensions
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services)
    {
        services.AddOptions<DatabaseOptions>().BindConfiguration(DatabaseOptions.Name);
        services.AddOptions<CertificateOptions>().BindConfiguration(CertificateOptions.Name);
        services.AddOptions<DefaultOptions>().BindConfiguration(DefaultOptions.Name);
        return services;
    }

    public static IServiceCollection AddReverseProxyCore(this IServiceCollection services)
    {
        var provider = new ProxyConfigProvider();
        services.AddSingleton<IProxyConfigProvider>(provider);
        services.AddSingleton<IUpdateConfig>(provider);
        services.AddReverseProxy();
        return services;
    }

    public static IServiceCollection AddServerCertificateSelector(this IServiceCollection services)
    {
        services.TryAddSingleton<IServerCertificateSelector, ServerCertificateSelector>();
        return services;
    }
}