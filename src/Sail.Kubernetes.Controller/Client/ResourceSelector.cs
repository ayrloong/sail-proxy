using k8s;
using k8s.Models;

namespace Sail.Kubernetes.Controller.Client;

public class ResourceSelector<TResource>
    where TResource : class, IKubernetesObject<V1ObjectMeta>, new()
{
    public ResourceSelector(string fieldSelector)
    {
        FieldSelector = fieldSelector;
    }

    public string FieldSelector { get; }
}