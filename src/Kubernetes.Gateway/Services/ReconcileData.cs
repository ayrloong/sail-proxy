using Sail.Kubernetes.Gateway.Caching;

namespace Sail.Kubernetes.Gateway.Services;

public struct ReconcileData
{
    public ReconcileData(GatewayData gateway, List<ServiceData> services, List<Endpoints> endpoints)
    {
        Gateway = gateway;
        ServiceList = services;
        EndpointsList = endpoints;
    }

    public GatewayData Gateway { get; }
    public List<ServiceData> ServiceList { get; }
    public List<Endpoints> EndpointsList { get; }
}