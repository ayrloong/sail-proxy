using Sail.Kubernetes.Controller.Caching;

namespace Sail.Kubernetes.Controller.Converters;

public class GatewayContext
{
    public GatewayContext(GatewayData gateway, List<HttpRouteData> httpRoutes, List<ServiceData> services,
        List<Endpoints> endpoints)
    {
        Gateway = gateway;
        HttpRoutes = httpRoutes;
        Services = services;
        Endpoints = endpoints;
    }

    public GatewayOptions Options { get; } = new();
    public GatewayData Gateway { get; }
    public List<HttpRouteData> HttpRoutes { get; }
    public List<ServiceData> Services { get; }
    public List<Endpoints> Endpoints { get; }
}