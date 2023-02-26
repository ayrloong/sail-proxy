using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;

namespace Sail.Kubernetes.Protocol.Options;

public class CorsOptionsUpdater : ICorsOptionsUpdater
{
    private readonly CorsOptions _options;

    public CorsOptionsUpdater(IOptions<CorsOptions> options)
    {
        _options = options.Value;
    }

    public Task UpdateAsync(string name, List<string> allowOrigins, List<string> allowMethods,
        List<string> allowHeaders)
    {
        _options.AddPolicy(name, policy =>
        {
            policy.WithOrigins(allowOrigins.ToArray());
            policy.WithMethods(allowMethods.ToArray());
            policy.WithHeaders(allowHeaders.ToArray());

        });
        return Task.CompletedTask;
    }
}