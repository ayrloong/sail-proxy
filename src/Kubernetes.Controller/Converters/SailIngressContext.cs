using Sail.Kubernetes.Controller.Caching;

namespace Sail.Kubernetes.Controller.Converters;

public class SailIngressContext
{
    public SailIngressContext(IngressData ingress, List<ServiceData> services, List<Endpoints> endpoints)
    {
        Ingress = ingress;
        Services = services;
        Endpoints = endpoints;
    }

    public SailIngressOptions Options { get; set; } = new();
    public IngressData Ingress { get; }
    public List<ServiceData> Services { get; }
    public List<Endpoints> Endpoints { get; }
}