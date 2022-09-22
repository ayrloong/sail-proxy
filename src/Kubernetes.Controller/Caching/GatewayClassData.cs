using k8s.Models;
using Sail.Kubernetes.Controller.Models;

namespace Sail.Kubernetes.Controller.Caching;

public class GatewayClassData
{
    public GatewayClassData(V1beta1GatewayClass gatewayClass)
    {
        GatewayClass = gatewayClass;
        IsDefault = GetDefaultAnnotation(gatewayClass);
    }

    public V1beta1GatewayClass GatewayClass { get; }
    public bool IsDefault { get; }

    private static bool GetDefaultAnnotation(V1beta1GatewayClass gatewayClass)
    {
        var annotation = gatewayClass.GetAnnotation("gatewayclass.kubernetes.io/is-default-class");
        return string.Equals("true", annotation, StringComparison.OrdinalIgnoreCase);
    }
}