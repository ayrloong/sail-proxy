using k8s;
using k8s.Models;

namespace Sail.Kubernetes.Controller.Client;


public delegate void ResourceInformerCallback<TResource>(WatchEventType eventType, TResource resource) where TResource : class, IKubernetesObject<V1ObjectMeta>;

public interface IResourceInformer<TResource> : IHostedService, IResourceInformer
    where TResource : class, IKubernetesObject<V1ObjectMeta>, new()
{
    IResourceInformerRegistration Register(ResourceInformerCallback<TResource> callback);
}

public interface IResourceInformer
{
    void StartWatching();
    Task ReadyAsync(CancellationToken cancellationToken);
    IResourceInformerRegistration Register(ResourceInformerCallback<IKubernetesObject<V1ObjectMeta>> callback);
}