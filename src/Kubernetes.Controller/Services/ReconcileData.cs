using Sail.Kubernetes.Controller.Caching;

namespace Sail.Kubernetes.Controller.Services;

public struct ReconcileData
{
    public ReconcileData(GatewayData gateway, List<HttpRouteData> httpRoutes,
        List<ServiceData> services, List<Endpoints> endpoints)
    {
        Gateway = gateway;
        HttpRouteList = httpRoutes;
        ServiceList = services;
        EndpointsList = endpoints;
    }

    public GatewayData Gateway { get; }
    public List<HttpRouteData> HttpRouteList { get; }
    public List<ServiceData> ServiceList { get; }
    public List<Endpoints> EndpointsList { get; }
}