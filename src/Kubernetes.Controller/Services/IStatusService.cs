using Sail.Kubernetes.Controller.Models;

namespace Sail.Kubernetes.Controller.Services;

public interface IStatusService
{
    Task PatchGatewayClassStatusAsync(V1beta1GatewayClass gatewayClass, CancellationToken cancellationToken);
}