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

        app.MapRouteApiV1().WithTags("Routes API").RequireHost(host);
        app.MapClusterApiV1().WithTags("Clusters API").RequireHost(host);
        app.MapCertificateApiV1().WithTags("Certificates API").RequireHost(host);
        
        return app;
    }
}