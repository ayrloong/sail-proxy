using Sail.Kubernetes.Gateway.Dispatching;

namespace Sail.Kubernetes.Gateway.Services;

public interface IReconciler
{
    void OnAttach(Action<IDispatchTarget> attached);
    Task ProcessAsync(CancellationToken cancellationToken);
}