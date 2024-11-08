using k8s;
using k8s.Autorest;
using k8s.Models;

namespace Sail.Kubernetes.Controller.Client;

internal class V1EndpointsResourceInformer(
    IKubernetes client,
    ResourceSelector<V1Endpoints> selector,
    IHostApplicationLifetime hostApplicationLifetime,
    ILogger<V1EndpointsResourceInformer> logger)
    : ResourceInformer<V1Endpoints, V1EndpointsList>(client, selector, hostApplicationLifetime, logger)
{
    protected override Task<HttpOperationResponse<V1EndpointsList>> RetrieveResourceListAsync(bool? watch = null,
        string resourceVersion = null, ResourceSelector<V1Endpoints> resourceSelector = null,
        CancellationToken cancellationToken = default)
    {
        return Client.CoreV1.ListEndpointsForAllNamespacesWithHttpMessagesAsync(watch: watch,
            resourceVersion: resourceVersion, fieldSelector: resourceSelector?.FieldSelector,
            cancellationToken: cancellationToken);
    }
}