using Microsoft.Extensions.Hosting;

namespace Sail.Compass.Informers;

public delegate void ResourceInformerCallback<TResource>(ResourceEvent<TResource> resource);

public interface IResourceInformer<TResource> : IHostedService, IResourceInformer
    where TResource : class
{
    IResourceInformerRegistration Register(ResourceInformerCallback<TResource> callback);
}
public interface IResourceInformer
{
    void StartWatching();
    Task ReadyAsync(CancellationToken cancellationToken);
}