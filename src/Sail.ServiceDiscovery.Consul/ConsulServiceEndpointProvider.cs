using System.Net;
using Consul;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.ServiceDiscovery;

namespace Sail.ServiceDiscovery.Consul;

internal sealed partial class ConsulServiceEndpointProvider(
    ServiceEndpointQuery query,
    string hostName,
    IOptionsMonitor<ConsulServiceEndpointProviderOptions> options,
    ILogger<ConsulServiceEndpointProvider> logger,
    ConsulClient client,
    TimeProvider timeProvider) :
    ConsulServiceEndpointProviderBase(query, logger, timeProvider), IHostNameFeature
{
    protected override double RetryBackOffFactor { get; }
    protected override TimeSpan MinRetryPeriod { get; }
    protected override TimeSpan MaxRetryPeriod { get; }
    protected override TimeSpan DefaultRefreshPeriod { get; }
    protected override async Task ResolveAsyncCore()
    {
        var endpoints = new List<ServiceEndpoint>();
        var ttl = DefaultRefreshPeriod;
        Log.AddressQuery(logger, ServiceName, hostName);
        var result = await client.Catalog.Service(hostName).ConfigureAwait(false);

        foreach (var service in result.Response)
        {
            var ipAddress = new IPAddress(service.ServiceAddress.Split('.').Select(a => Convert.ToByte(a)).ToArray());
            var ipPoint = new IPEndPoint(ipAddress, service.ServicePort);
            var serviceEndpoint = ServiceEndpoint.Create(ipPoint);
            serviceEndpoint.Features.Set<IServiceEndpointProvider>(this);
            endpoints.Add(serviceEndpoint);
        }
        
        if (endpoints.Count == 0)
        {
            throw new InvalidOperationException($"No records were found for service '{ServiceName}' ( name: '{hostName}').");
        }

        SetResult(endpoints, ttl);
    }

    public string HostName { get; }
}