using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Sail.Kubernetes.Protocol.Configuration;

public class AuthenticationSchemeUpdater : IAuthenticationSchemeUpdater
{
    private readonly IAuthenticationSchemeProvider _provider;
    private readonly IOptionsMonitorCache<JwtBearerOptions> _options;
    private readonly AuthorizationOptions _authorizationOptions;

    public AuthenticationSchemeUpdater(IAuthenticationSchemeProvider provider,
        IOptionsMonitorCache<JwtBearerOptions> options,
        IOptions<AuthorizationOptions> authorizationOptions)
    {
        _provider = provider;
        _options = options;
        _authorizationOptions = authorizationOptions.Value;
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
        _authorizationOptions.AddPolicy(name, policy => { policy.RequireAuthenticatedUser(); });
        return Task.CompletedTask;
    }
}