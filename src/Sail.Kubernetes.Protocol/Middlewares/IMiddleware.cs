using Sail.Kubernetes.Protocol.Configuration;

namespace Sail.Kubernetes.Protocol.Middlewares;

public interface IMiddleware
{
    Task ApplyAsync(IEnumerable<MiddlewareConfig> middlewares);
}