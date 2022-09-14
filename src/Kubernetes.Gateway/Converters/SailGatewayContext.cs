using Sail.Kubernetes.Gateway.Caching;

namespace Sail.Kubernetes.Gateway.Converters;

public class SailGatewayContext
{
    public SailGatewayContext(GatewayData gateway, List<ServiceData> services, List<Endpoints> endpoints)
    {
        Gateway = gateway;
        Services = services;
        Endpoints = endpoints;
    }

    public SailGatewayOptions Options { get; set; } = new();
    public GatewayData Gateway { get; }
    public HttpRouteData HttpRoute { get; set; }
    public List<ServiceData> Services { get; }
    public List<Endpoints> Endpoints { get; }
}