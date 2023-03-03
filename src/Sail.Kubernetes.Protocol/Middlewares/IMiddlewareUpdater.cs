using Sail.Kubernetes.Protocol.Configuration;

namespace Sail.Kubernetes.Protocol.Middlewares;

public interface IMiddlewareUpdater
{
    Task UpdateAsync(IEnumerable<MiddlewareConfig> middlewareConfigs);
}