using Microsoft.Extensions.DependencyInjection;
using Yarp.ReverseProxy.Configuration;

namespace Sail.Kubernetes.Protocol;

public static class MessageConfigProviderExtensions
{
    public static IReverseProxyBuilder LoadFromMessages(this IReverseProxyBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        var provider = new MessageConfigProvider();
        builder.Services.AddSingleton<IProxyConfigProvider>(provider);
        builder.Services.AddSingleton<IUpdateConfig>(provider);
        return builder;
    }
}