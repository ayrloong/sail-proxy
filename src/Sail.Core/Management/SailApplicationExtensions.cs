using Microsoft.Extensions.DependencyInjection;
using Sail.Core.Configuration.ConfigProvider;
using Sail.Core.Hosting;
using Yarp.ReverseProxy.Configuration;

namespace Sail.Core.Management;

public static class SailApplicationExtensions
{
    public static SailApplication AddSailCore(this IServiceCollection services)
    {
        var app = new SailApplication(services);

        services.AddHostedService<ProxyHostedService>();
        services.AddHostedService<CertificateHostedService>();
        services.AddServerCertificateSelector().AddConfiguration();
        services.AddReverseProxy().LoadFromMessages();
        return app;
    }

    private static IReverseProxyBuilder LoadFromMessages(this IReverseProxyBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        var provider = new ProxyConfigProvider();
        builder.Services.AddSingleton<IProxyConfigProvider>(provider);
        builder.Services.AddSingleton<IUpdateConfig>(provider);
        return builder;
    }
}