using Microsoft.Extensions.DependencyInjection;

namespace Sail.Kubernetes.Protocol;

public static class MessageConfigProviderExtensions
{
    public static IReverseProxyBuilder LoadFromMessages(this IReverseProxyBuilder builder)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }
        
        return builder;
    }
}