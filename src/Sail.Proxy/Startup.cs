using System.Text;
using Microsoft.IdentityModel.Tokens;
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
        services.AddAuthentication();
        services.AddCors();
        services.Configure<ReceiverOptions>(Configuration.Bind);
        services.AddHostedService<Receiver>();
        services.AddKubernetesMiddleware().AddReverseProxy().LoadFromMessages();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapReverseProxy(); });
    }
}