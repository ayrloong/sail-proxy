using Sail.Core.Options;
using Sail.Services;

namespace Sail.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        services.AddOptions<DatabaseOptions>()
            .BindConfiguration(DatabaseOptions.Name);

        services.AddTransient<IRouteService, RouteService>();
        services.AddTransient<IClusterService, ClusterService>();
        services.AddTransient<ICertificateService, CertificateService>();
    }
}