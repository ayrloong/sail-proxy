using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sail.Protocol.Apis;
using Sail.Protocol.Services;

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
        var configuration = app.Configuration;
        var host = configuration["Protocol:Api"] ?? string.Empty;

        var endpoint = app.NewVersionedApi();

        endpoint.MapRouteApiV1().WithTags("Routes API").RequireHost(host);
        endpoint.MapClusterApiV1().WithTags("Clusters API").RequireHost(host);
        endpoint.MapCertificateApiV1().WithTags("Certificates API").RequireHost(host);
        
        return app;
    }
}