using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;

namespace Sail.Kubernetes.Protocol.Configuration;

public class CorsOptionsUpdater : ICorsOptionsUpdater
{
    private readonly CorsOptions _options;

    public CorsOptionsUpdater(IOptions<CorsOptions> options)
    {
        _options = options.Value;
    }

    public Task UpdateAsync(CorsConfig cors)
    {
        _options.AddPolicy(cors.Name, policy =>
        {
            policy.WithOrigins(cors.AllowOrigins.ToArray());
            policy.WithMethods(cors.AllowMethods.ToArray());
            policy.WithHeaders(cors.AllowHeaders.ToArray());
        });
        return Task.CompletedTask;
    }
}