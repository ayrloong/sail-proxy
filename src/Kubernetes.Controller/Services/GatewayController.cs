using Microsoft.Kubernetes.Controller.Hosting;

namespace Sail.Kubernetes.Controller.Services;

public class GatewayController:BackgroundHostedService
{
    public GatewayController(IHostApplicationLifetime hostApplicationLifetime, ILogger logger)
        : base(hostApplicationLifetime, logger)
    {
        
    }

    public override Task RunAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}