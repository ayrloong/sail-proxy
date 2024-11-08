using Sail.Kubernetes.Controller.Hosting;

namespace Sail.Kubernetes.Controller.Services;

public class GatewayController(IHostApplicationLifetime hostApplicationLifetime, ILogger logger)
    : BackgroundHostedService(hostApplicationLifetime, logger)
{
    public override Task RunAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}