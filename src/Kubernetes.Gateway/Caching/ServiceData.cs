using k8s.Models;

namespace Sail.Kubernetes.Gateway.Caching;

public struct ServiceData
{
    public ServiceData(V1Service service)
    {
        if (service is null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        Spec = service.Spec;
        Metadata = service.Metadata;
    }

    public V1ServiceSpec Spec { get; set; }
    public V1ObjectMeta Metadata { get; set; }
}