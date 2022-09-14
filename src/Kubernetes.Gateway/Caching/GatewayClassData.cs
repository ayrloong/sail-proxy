using k8s.Models;
using Sail.Kubernetes.Gateway.Models;

namespace Sail.Kubernetes.Gateway.Caching;

public struct GatewayClassData
{
    public GatewayClassData(V1beta1GatewayClass gatewayClass)
    {
        if (gatewayClass is null)
        {
            throw new ArgumentNullException(nameof(gatewayClass));
        }

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