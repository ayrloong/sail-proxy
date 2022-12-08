using k8s;
using Microsoft.Extensions.Options;
using Microsoft.Kubernetes.Client;
using Sail.Kubernetes.Controller.Caching;

namespace Sail.Kubernetes.Controller.Services;

public class V1beta1GatewayClassResourceStatusUpdater : IGatewayClassResourceStatusUpdater
{
    private readonly IAnyResourceKind _resource;
    private readonly SailOptions _options;
    private readonly ICache _cache;
    private readonly ILogger _logger;

    public V1beta1GatewayClassResourceStatusUpdater(
        IAnyResourceKind resource,
        IOptions<SailOptions> options,
        ICache cache,
        ILogger<V1beta1GatewayClassResourceStatusUpdater> logger)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _resource = resource;
        _cache = cache;
        _logger = logger;
    }

    public Task UpdateStatusAsync()
    {
        throw new NotImplementedException();
    }
}