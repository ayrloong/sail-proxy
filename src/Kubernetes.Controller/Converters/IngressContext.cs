using Sail.Kubernetes.Controller.Caching;

namespace Sail.Kubernetes.Controller.Converters;

public class IngressContext
{
    public IngressContext(IngressData ingress, List<ServiceData> services, List<Endpoints> endpoints)
    {
        Ingress = ingress;
        Services = services;
        Endpoints = endpoints;
    }

    public IngressOptions Options { get; set; } = new();
    public IngressData Ingress { get; }
    public List<ServiceData> Services { get; }
    public List<Endpoints> Endpoints { get; }
}