using Sail.Kubernetes.Controller.Caching;

namespace Sail.Kubernetes.Controller.Converters;

internal sealed class SailIngressContext
{
    public SailIngressContext(IngressData ingress, List<ServiceData> services, List<Endpoints> endpoints,List<MiddlewareData> middlewares)
    {
        Ingress = ingress;
        Services = services;
        Endpoints = endpoints;
        Middlewares = middlewares;
    }

    public SailIngressOptions Options { get; set; } = new();
    public IngressData Ingress { get; }
    public List<MiddlewareData> Middlewares { get; set; }
    public List<ServiceData> Services { get; }
    public List<Endpoints> Endpoints { get; }
}