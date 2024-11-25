using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.ServiceDiscovery;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.ServiceDiscovery;

namespace Sail.Core.ServiceDiscovery;

public class ServiceDiscoveryDestinationResolver(ServiceEndpointResolver resolver)
    : IServiceDiscoveryDestinationResolver
{
    public async ValueTask<ResolvedDestinationCollection> ResolveDestinationsAsync(string serviceName,
        CancellationToken cancellationToken)
    {
        var result = await resolver.GetEndpointsAsync(serviceName, cancellationToken).ConfigureAwait(false);

        var destinations = new Dictionary<string, DestinationConfig>();
        foreach (var endpoint in result.Endpoints)
        {
            
        }

        return new ResolvedDestinationCollection(destinations, new CompositeChangeToken(new List<IChangeToken>()));
    }
}