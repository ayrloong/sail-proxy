using k8s.Models;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Sail.Kubernetes.Controller;
using Sail.Kubernetes.Controller.Caching;
using Sail.Kubernetes.Controller.Dispatching;
using Sail.Kubernetes.Controller.Models;
using Sail.Kubernetes.Controller.Services;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

var builder = WebApplication.CreateBuilder(args);

using var serilog = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .Enrich.FromLogContext()
               .WriteTo.Console(theme: AnsiConsoleTheme.Code)
               .CreateLogger();

builder.WebHost.ConfigureLogging(options =>
{
    options.ClearProviders();
    options.AddSerilog(serilog, dispose: false);
}).ConfigureAppConfiguration(config =>
{
    config.AddJsonFile("/app/config/sail.json", optional: true);
});

builder.Services.AddControllers();
builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());
builder.Services.AddKubernetesControllerRuntime();
builder.Services.AddHostedService<GatewayController>();
builder.Services.AddSingleton<ICache, GatewayCache>();
builder.Services.AddTransient<IReconciler, Reconciler>();
builder.Services.AddSingleton<IDispatcher, Dispatcher>();
builder.Services.Configure<SailOptions>(builder.Configuration.GetSection("Sail"));
builder.Services.RegisterResourceInformer<V1beta1Gateway>();
builder.Services.RegisterResourceInformer<V1beta1GatewayClass>();
builder.Services.RegisterResourceInformer<V1beta1HttpRoute>();
builder.Services.RegisterResourceInformer<V1Service>();
builder.Services.RegisterResourceInformer<V1Endpoints>();

var app = builder.Build();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHealthChecks("/healthz", new HealthCheckOptions
    {
        Predicate = r => r.Name.Contains("self")
    });
    endpoints.MapDefaultControllerRoute();
});

app.Run();