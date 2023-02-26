namespace Sail.Kubernetes.Protocol.Options;

public interface IAuthorizationOptionsUpdater
{
    Task UpdateAsync(string name);
}