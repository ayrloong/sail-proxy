using Sail.Kubernetes.Protocol.Configuration;

namespace Sail.Kubernetes.Protocol.Middlewares;

public class MiddlewareUpdater : IMiddlewareUpdater
{
    private readonly IEnumerable<IMiddleware> _middlewares;

    public MiddlewareUpdater(IEnumerable<IMiddleware> middlewares)
    {
        _middlewares = middlewares;
    }

    public Task UpdateAsync(IEnumerable<MiddlewareConfig> middlewareConfigs)
    {
        foreach (var middleware in _middlewares)
        {
            middleware.ApplyAsync(middlewareConfigs);
        }

        return Task.CompletedTask;
    }
}