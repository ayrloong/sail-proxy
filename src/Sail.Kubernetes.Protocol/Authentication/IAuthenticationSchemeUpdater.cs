namespace Sail.Kubernetes.Protocol.Authentication;

public interface IAuthenticationSchemeUpdater
{
    Task UpdateAsync(string name);
}