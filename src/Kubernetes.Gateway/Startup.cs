using k8s.Models;
using Sail.Kubernetes.Gateway.Caching;
using Sail.Kubernetes.Gateway.Dispatching;
using Sail.Kubernetes.Gateway.Models;
using Sail.Kubernetes.Gateway.Services;

namespace Sail.Kubernetes.Gateway;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Add components from the kubernetes controller framework
        services.AddKubernetesControllerRuntime();

        // Add components implemented by this application
        services.AddHostedService<GatewayController>();
        services.AddSingleton<ICache, GatewayCache>();
        services.AddTransient<IReconciler, Reconciler>();
        services.AddSingleton<IDispatcher, Dispatcher>();
        services.Configure<SailOptions>(_configuration.GetSection("Sail"));

        // Register the necessary Kubernetes resource informers
        services.RegisterResourceInformer<V1beta1Gateway>();
        services.RegisterResourceInformer<V1beta1HttpRoute>();
        services.RegisterResourceInformer<V1beta1GatewayClass>();
        services.RegisterResourceInformer<V1Endpoints>();
        services.RegisterResourceInformer<V1Service>();

        // Add ASP.NET Core controller support
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}