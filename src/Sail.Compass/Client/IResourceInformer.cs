using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Sail.Api.V1;

namespace Sail.Compass.Client;

public delegate void ResourceInformerCallback<TResource>(TResource resource);

public interface IResourceInformer<TResource> : IHostedService, IResourceInformer
{
    IResourceInformerRegistration Register(ResourceInformerCallback<TResource> callback);
}

public interface IResourceInformer
{
    void StartWatching();
    Task ReadyAsync(CancellationToken cancellationToken);
}
