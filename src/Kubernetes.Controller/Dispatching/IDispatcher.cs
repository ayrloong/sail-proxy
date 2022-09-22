namespace Sail.Kubernetes.Controller.Dispatching;

public interface IDispatcher
{
    void Attach(IDispatchTarget target);
    void Detach(IDispatchTarget target);
    void OnAttach(Action<IDispatchTarget> attached);
    Task SendAsync(IDispatchTarget specificTarget, byte[] utf8Bytes, CancellationToken cancellationToken);
}