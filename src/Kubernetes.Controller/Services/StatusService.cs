using System.Reflection;
using k8s;
using k8s.Models;
using Microsoft.Kubernetes.Client;
using Sail.Kubernetes.Controller.Models;

namespace Sail.Kubernetes.Controller.Services;

public class StatusService : IStatusService
{
    private readonly IAnyResourceKind _client;
    private readonly ILogger<StatusService> _logger;
    public StatusService(IKubernetes client, ILogger<StatusService> logger)
    {
        _client = client.AnyResourceKind();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task PatchGatewayClassStatusAsync(V1beta1GatewayClass gatewayClass,
        CancellationToken cancellationToken)
    {

        var status = new V1beta1GatewayClassStatus
        {
            Conditions = new List<V1beta1GatewayClassStatusCondition>
            {
                new()
                {
                    Reason = "True",
                    Status = "True",
                    Message = "Valid GatewayClass",
                    Type = "Accepted",
                    LastTransitionTime = DateTime.Now
                }
            }
        };
        var kubernetesEntity = gatewayClass.GetType().GetCustomAttribute<KubernetesEntityAttribute>();

        using var response = await _client.PatchAnyResourceKindWithHttpMessagesAsync(
            body: new V1Patch(new V1beta1GatewayClass { Status = status }, V1Patch.PatchType.MergePatch),
            group: kubernetesEntity.Group,
            version: kubernetesEntity.ApiVersion,
            namespaceParameter: gatewayClass.Namespace(),
            plural: kubernetesEntity.PluralName,
            name: gatewayClass.Name(),
            cancellationToken: cancellationToken);

    }
}