using k8s.Models;
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

// Add ASP.NET Core controller support
builder.Services.AddControllers();

// Add components from the kubernetes controller framework
builder.Services.AddKubernetesControllerRuntime();

// Add components implemented by this application
builder.Services.AddHostedService<IngressController>();
builder.Services.AddSingleton<ICache, IngressCache>();
builder.Services.AddTransient<IReconciler, Reconciler>();
builder.Services.AddSingleton<IDispatcher, Dispatcher>();
builder.Services.Configure<SailOptions>(builder.Configuration.GetSection("Sail"));

// Register the necessary Kubernetes resource informers
builder.Services.RegisterResourceInformer<V1Ingress>();
builder.Services.RegisterResourceInformer<V1Service>();
builder.Services.RegisterResourceInformer<V1Endpoints>();
builder.Services.RegisterResourceInformer<V1IngressClass>();
builder.Services.RegisterResourceInformer<V1beta1Middleware>();

var app = builder.Build();
app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});

app.Run();