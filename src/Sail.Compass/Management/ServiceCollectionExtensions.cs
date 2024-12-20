using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sail.Api.V1;
using Sail.Compass.Client;
using Sail.Compass.Hosting;
using Sail.Compass.Services;
using Sail.Core.Certificates;

namespace Sail.Compass.Management;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddControllerRuntime(this IServiceCollection services)
    {

        services.AddHostedService<ProxyDiscoveryService>();
        services.RegisterResourceInformer<RouteItems, V1RouteResourceInformer>();
        services.RegisterResourceInformer<ClusterItems, V1ClusterResourceInformer>();
        return services;
    }

    public static IServiceCollection RegisterResourceInformer<TResource, TService>(this IServiceCollection services)
        where TResource : class
        where TService : IResourceInformer<TResource>
    {
        services.AddSingleton(typeof(IResourceInformer<TResource>), typeof(TService)); 

        return services.RegisterHostedService<IResourceInformer<TResource>>();
    }
}