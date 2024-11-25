using Sail.Apis;
using Sail.Services;

namespace Microsoft.AspNetCore.Builder;

public static class SailApplicationBuilderExtensions
{

    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails();
        builder.Services.AddTransient<IRouteService, RouteService>();
        builder.Services.AddTransient<IClusterService, ClusterService>();
        builder.Services.AddTransient<ICertificateService, CertificateService>();

    }

    public static IApplicationBuilder MapSailApiService(this WebApplication app)
    {

        var endpoint = app.NewVersionedApi();
        
        endpoint.MapRouteApiV1().WithTags("Routes API");
        endpoint.MapClusterApiV1().WithTags("Clusters API");
        endpoint.MapCertificateApiV1().WithTags("Certificates API");

        return app;
    }
}