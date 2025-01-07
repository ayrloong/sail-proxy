namespace Sail.Compass.Informers;

public interface IResourceInformerRegistration : IDisposable
{
    Task ReadyAsync(CancellationToken cancellationToken);
}