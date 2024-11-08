using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sail.Protocol.Apis;
using Sail.Protocol.Grpc;
using Sail.Protocol.Services;

namespace Microsoft.AspNetCore.Builder;

public static class SailApplicationBuilderExtensions
{
    
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddGrpc();
        builder.Services.AddTransient<IRouteService, Sail.Protocol.Services.RouteService>();
    }
    
    public static IApplicationBuilder MapSailGrpcService(this WebApplication app)
    {
        var configuration = app.Configuration;
        var host = configuration["Protocol:Grpc"] ?? string.Empty;

        app.MapGrpcService<CertificateServiceGrpc>().RequireHost(host);
        app.MapGrpcService<ClusterServiceGrpc>().RequireHost(host);
        app.MapGrpcService<RouteServiceGrpc>().RequireHost(host);

        return app;
    }

    public static IApplicationBuilder MapSailApiService(this WebApplication app)
    {
        var configuration = app.Configuration;
        var host = configuration["Protocol:Api"] ?? string.Empty;

        app.MapGroup("/api/route")
            .WithTags("Route API")
            .RequireHost(host)
            .MapRouteApiV1();

        app.MapGroup("/api/cluster")
            .WithTags("Cluster API")
            .RequireHost(host)
            .MapClusterApiV1();

        app.MapGroup("/api/certificate")
            .WithTags("Certificate API")
            .RequireHost(host)
            .MapCertificateApiV1();

        return app;
    }
}