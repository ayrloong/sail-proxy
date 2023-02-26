namespace Sail.Kubernetes.Protocol.Options;

public interface IRateLimiterOptionsUpdater
{
    Task UpdateAsync(string name);
}