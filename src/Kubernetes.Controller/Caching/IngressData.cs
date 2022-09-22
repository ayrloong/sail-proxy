using k8s.Models;

namespace Sail.Kubernetes.Controller.Caching;

public struct IngressData
{
    public IngressData(V1Ingress ingress)
    {
        if (ingress is null)
        {
            throw new ArgumentNullException(nameof(ingress));
        }

        Spec = ingress.Spec;
        Metadata = ingress.Metadata;
    }
    
    public V1IngressSpec Spec { get; set; }
    public V1ObjectMeta Metadata { get; set; }
}