using Sail.Kubernetes.Controller.Caching;

namespace Sail.Kubernetes.Controller.Services;

public struct ReconcileData
{
    public ReconcileData(IngressData ingress, List<ServiceData> services, List<Endpoints> endpoints)
    {
        Ingress = ingress;
        ServiceList = services;
        EndpointsList = endpoints;
    }

    public IngressData Ingress { get; }
    public List<ServiceData> ServiceList { get; }
    public List<Endpoints> EndpointsList { get; }
}