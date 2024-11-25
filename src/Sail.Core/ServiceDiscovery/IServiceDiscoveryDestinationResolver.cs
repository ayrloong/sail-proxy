using Yarp.ReverseProxy.ServiceDiscovery;

namespace Sail.Core.ServiceDiscovery;

public interface IServiceDiscoveryDestinationResolver
{
    ValueTask<ResolvedDestinationCollection> ResolveDestinationsAsync(string serviceName,
        CancellationToken cancellationToken);
}