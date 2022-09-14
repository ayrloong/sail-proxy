using k8s.Models;

namespace Sail.Kubernetes.Controller.Caching;

public struct IngressClassData
{
    public IngressClassData(V1IngressClass ingressClass)
    {
        if (ingressClass is null)
        {
            throw new ArgumentNullException(nameof(ingressClass));
        }

        IngressClass = ingressClass;
        IsDefault = GetDefaultAnnotation(ingressClass);
    }

    public V1IngressClass IngressClass { get; }

    public bool IsDefault { get; }

    private static bool GetDefaultAnnotation(V1IngressClass ingressClass)
    {
        var annotation = ingressClass.GetAnnotation("ingressclass.kubernetes.io/is-default-class");
        return string.Equals("true", annotation, StringComparison.OrdinalIgnoreCase);
    }
}   