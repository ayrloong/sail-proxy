using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Sail.Kubernetes.Protocol.Authentication;

public class AuthenticationSchemeUpdater : IAuthenticationSchemeUpdater
{
    private readonly IAuthenticationSchemeProvider _provider;
    private readonly IOptionsMonitorCache<JwtBearerOptions> _options;

    public AuthenticationSchemeUpdater(IAuthenticationSchemeProvider provider,
        IOptionsMonitorCache<JwtBearerOptions> options)
    {
        _provider = provider;
        _options = options;
    }

    public Task UpdateAsync(string name)
    {
        var scheme = new AuthenticationScheme(name, displayName: null, typeof(JwtBearerHandler));
        _provider.AddScheme(scheme);
        _options.TryAdd(name, new JwtBearerOptions
        {
            RequireHttpsMetadata = false,
            SaveToken = true,
        });
        return Task.CompletedTask;
    }
}