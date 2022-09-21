using Sail.Kubernetes.Controller.Caching;

namespace Sail.Kubernetes.Controller.Converters;

public class GatewayContext
{
    public GatewayContext(GatewayData gateway, List<HttpRouteData> httpRoutes)
    {
        Gateway = gateway;
        HttpRoutes = httpRoutes;
    }

    public GatewayOptions Options { get; set; } = new();
    public GatewayData Gateway { get; }
    public List<ServiceData> Services { get; }
    public List<HttpRouteData> HttpRoutes { get; }
}