using Yarp.ReverseProxy.ServiceDiscovery;

namespace Yarp.Extensions.Resilience.ServiceDiscovery;

public interface IServiceDiscoveryDestinationResolver
{
    ValueTask<ResolvedDestinationCollection> ResolveDestinationsAsync(string serviceName,
        CancellationToken cancellationToken);
}