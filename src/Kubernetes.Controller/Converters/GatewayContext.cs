using Sail.Kubernetes.Controller.Caching;

namespace Sail.Kubernetes.Controller.Converters;

public class GatewayContext
{
    public List<HttpRouteData> HttpRoutes { get; }
}