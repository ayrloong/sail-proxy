using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

namespace ServiceDefaults;

public static partial class Extensions
{
    public static IApplicationBuilder UseDefaultOpenApi(this WebApplication app)
    {
        var configuration = app.Configuration;
        var openApiSection = configuration.GetSection("OpenApi");
        var host = configuration["Protocol:Api"] ?? string.Empty;

        if (!openApiSection.Exists())
        {
            return app;
        }

        app.MapOpenApi().RequireHost(host);
        app.MapScalarApiReference().RequireHost(host);

        return app;
    }

    public static IHostApplicationBuilder AddDefaultOpenApi(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        var openApi = configuration.GetSection("OpenApi");

        if (!openApi.Exists())
        {
            return builder;
        }

        services.AddOpenApi();
        return builder;
    }
}