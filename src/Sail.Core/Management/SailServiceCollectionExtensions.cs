using Sail.Core.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class SailServiceCollectionExtensions
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services)
    {
        services.AddOptions<DatabaseOptions>().BindConfiguration(DatabaseOptions.Name);
        services.AddOptions<CertificateOptions>().BindConfiguration(CertificateOptions.Name);
        return services;
    }
}