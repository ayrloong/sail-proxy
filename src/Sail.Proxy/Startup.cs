using Sail.Kubernetes.Protocol;

namespace Sail.Proxy;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<ReceiverOptions>(Configuration.Bind);
        services.AddUpdater();
        services.AddHostedService<Receiver>();
        services.AddReverseProxy().LoadFromMessages();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseCors();
        app.UseRateLimiter();
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseEndpoints(endpoints => { endpoints.MapReverseProxy(); });
    }
}