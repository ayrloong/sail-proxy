using Sail.Kubernetes.Controller.Dispatching;

namespace Sail.Kubernetes.Controller.Services;

public interface IReconciler
{
    void OnAttach(Action<IDispatchTarget> attached);
    Task ProcessAsync(CancellationToken cancellationToken);
}