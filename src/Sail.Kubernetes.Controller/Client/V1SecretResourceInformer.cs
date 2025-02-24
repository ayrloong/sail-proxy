using k8s;
using k8s.Autorest;
using k8s.Models;

namespace Sail.Kubernetes.Controller.Client;

internal class V1SecretResourceInformer(
    IKubernetes client,
    ResourceSelector<V1Secret> selector,
    IHostApplicationLifetime hostApplicationLifetime,
    ILogger<V1SecretResourceInformer> logger)
    : ResourceInformer<V1Secret, V1SecretList>(client, selector, hostApplicationLifetime, logger)
{
    protected override Task<HttpOperationResponse<V1SecretList>> RetrieveResourceListAsync(bool? watch = null,
        string resourceVersion = null, ResourceSelector<V1Secret> resourceSelector = null,
        CancellationToken cancellationToken = default)
    {
        return Client.CoreV1.ListSecretForAllNamespacesWithHttpMessagesAsync(watch: watch,
            resourceVersion: resourceVersion, fieldSelector: resourceSelector?.FieldSelector,
            cancellationToken: cancellationToken);
    }
}