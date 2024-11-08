namespace Sail.Kubernetes.Controller.Client;

public interface IResourceInformerRegistration : IDisposable
{
    Task ReadyAsync(CancellationToken cancellationToken);
}