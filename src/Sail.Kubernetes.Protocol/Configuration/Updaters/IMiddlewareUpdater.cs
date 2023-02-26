namespace Sail.Kubernetes.Protocol.Configuration;

public interface IMiddlewareUpdater
{
    Task UpdateAsync(IReadOnlyList<MiddlewareConfig> middlewares);
}