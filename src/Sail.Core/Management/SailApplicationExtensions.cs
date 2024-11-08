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
        services.AddConfiguration();
        
        services.AddReverseProxy().LoadFromMessages();
        return app;
    }
    
    public static IReverseProxyBuilder LoadFromMessages(this IReverseProxyBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }
        
        var provider = new  ProxyConfigProvider();
        builder.Services.AddSingleton<IProxyConfigProvider>(provider);
        builder.Services.AddSingleton<IUpdateConfig>(provider);
        return builder;
    }
}