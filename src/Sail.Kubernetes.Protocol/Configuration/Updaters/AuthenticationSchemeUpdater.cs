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

    public Task UpdateAsync(JwtBearerConfig jwtBearer)
    {
        var scheme = new AuthenticationScheme(jwtBearer.Name, displayName: null, typeof(JwtBearerHandler));
        _provider.AddScheme(scheme);
        _options.TryAdd(jwtBearer.Name, new JwtBearerOptions
        {
            Audience = jwtBearer.Audience,
            Authority = jwtBearer.Issuer,
            RequireHttpsMetadata = false
        });
        _authorizationOptions.AddPolicy(jwtBearer.Name, policy => { policy.RequireAuthenticatedUser(); });
        return Task.CompletedTask;
    }
}