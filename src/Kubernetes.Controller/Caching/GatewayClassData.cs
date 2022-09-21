using Sail.Kubernetes.Controller.Models;

namespace Sail.Kubernetes.Controller.Caching;

public class GatewayClassData
{
    public GatewayClassData(V1beta1GatewayClass gatewayClass)
    {
        GatewayClass = gatewayClass;
    }

    public V1beta1GatewayClass GatewayClass { get; }

}