namespace Sail.Kubernetes.Controller.Services;

public interface IGatewayClassResourceStatusUpdater
{
    Task UpdateStatusAsync();
}