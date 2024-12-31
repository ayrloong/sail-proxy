namespace Sail.Compass.Services;

public interface IReconciler
{
    Task ProcessAsync(CancellationToken cancellationToken);
}