using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;
using Sail.Kubernetes.Protocol.Configuration;

namespace Sail.Kubernetes.Protocol.Middlewares;

public class CorsMiddleware : IMiddleware
{
    private readonly CorsOptions _options;

    public CorsMiddleware(IOptions<CorsOptions> options)
    {
        _options = options.Value;
    }

    public Task ApplyAsync(IEnumerable<MiddlewareConfig> middlewares)
    {
        var corsPolicies = middlewares.Where(x => x.Cors is not null).Select(x => x.Cors);

        foreach (var cors in corsPolicies)
        {
            _options.AddPolicy(cors.Name, policy =>
            {
                if (cors.AllowOrigins is not null)
                {
                    policy.WithOrigins(cors.AllowOrigins.ToArray());
                }

                if (cors.AllowMethods is not null)
                {
                    policy.WithMethods(cors.AllowMethods.ToArray());
                }

                if (cors.AllowHeaders is not null)
                {
                    policy.WithHeaders(cors.AllowHeaders.ToArray());
                }
            });
        }

        return Task.CompletedTask;
    }
}