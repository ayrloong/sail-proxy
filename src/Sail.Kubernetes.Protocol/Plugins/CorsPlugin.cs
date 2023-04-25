using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;
using Sail.Kubernetes.Protocol.Configuration;

namespace Sail.Kubernetes.Protocol.Plugins;

public class CorsPlugin : IPlugin
{
    private readonly CorsOptions _options;

    public CorsPlugin(IOptions<CorsOptions> options)
    {
        _options = options.Value;
    }

    public Task ApplyAsync(IEnumerable<PluginConfig> plugins)
    {
        var corsPolicies = plugins.Where(x => x.Cors is not null).Select(x => x.Cors);

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