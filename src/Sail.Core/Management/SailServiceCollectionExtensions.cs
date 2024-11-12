using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sail.Core.Certificates;
using Sail.Core.Options;

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

    public static IServiceCollection AddServerCertificateSelector(this IServiceCollection services)
    {
        services.TryAddTransient<IServerCertificateSelector, ServerCertificateSelector>();
        return services;
    }
}