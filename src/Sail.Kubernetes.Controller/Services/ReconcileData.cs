using Sail.Kubernetes.Controller.Caching;

namespace Sail.Kubernetes.Controller.Services;

public struct ReconcileData
{
    public ReconcileData(IngressData ingress,List<ServiceData> services,List<Endpoints> endpoints)
    {
        Ingress = ingress;
        Services = services;
        EndpointsList = endpoints;
    }

    public IngressData Ingress { get;}
    public List<ServiceData> Services { get; set; }
    public List<Endpoints> EndpointsList { get; set; }
}