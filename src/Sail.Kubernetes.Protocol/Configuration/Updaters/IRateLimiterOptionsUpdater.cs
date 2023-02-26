namespace Sail.Kubernetes.Protocol.Configuration;

public interface IRateLimiterOptionsUpdater
{
    Task UpdateAsync(string name);
}