namespace Sail.Kubernetes.Protocol.Configuration;

public interface IMiddlewareUpdater
{
    Task UpdateAsync(List<MiddlewareConfig> middlewares);
}