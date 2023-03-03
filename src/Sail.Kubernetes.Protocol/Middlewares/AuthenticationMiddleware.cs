using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Sail.Kubernetes.Protocol.Configuration;

namespace Sail.Kubernetes.Protocol.Middlewares;

public class AuthenticationMiddleware : IMiddleware
{

    private readonly IAuthenticationSchemeProvider _provider;
    private readonly IOptionsMonitorCache<JwtBearerOptions> _options;
    private readonly AuthorizationOptions _authorizationOptions;

    public AuthenticationMiddleware(
        IAuthenticationSchemeProvider provider,
        IOptionsMonitorCache<JwtBearerOptions> options,
        IOptions<AuthorizationOptions> authorizationOptions)
    {
        _provider = provider;
        _options = options;
        _authorizationOptions = authorizationOptions.Value;
    }

    public Task ApplyAsync(IEnumerable<MiddlewareConfig> middlewares)
    {
        var jwtBearers = middlewares.Where(x => x.JwtBearer is not null).Select(x => x.JwtBearer);
        foreach (var jwtBearer in jwtBearers)
        {
            var scheme = new AuthenticationScheme(jwtBearer.Name, displayName: null, typeof(JwtBearerHandler));
            _provider.TryAddScheme(scheme);
            _options.TryAdd(jwtBearer.Name, new JwtBearerOptions
            {
                Audience = jwtBearer.Audience,
                Authority = jwtBearer.Issuer,
                RequireHttpsMetadata = false
            });
            _authorizationOptions.AddPolicy(jwtBearer.Name, policy => { policy.RequireAuthenticatedUser(); });
        }

        return Task.CompletedTask;
    }
}