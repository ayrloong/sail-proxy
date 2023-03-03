using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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
        _options.Clear();
    }

    public Task ApplyAsync(IEnumerable<MiddlewareConfig> middlewares)
    {

        var jwtBearers = middlewares.Where(x => x.JwtBearer is not null).Select(x => x.JwtBearer);
        foreach (var jwtBearer in jwtBearers)
        {
            var scheme =
                new AuthenticationScheme(jwtBearer.Name, displayName: jwtBearer.Name, typeof(JwtBearerHandler));
            _provider.TryAddScheme(scheme);
            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtBearer.Secret));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidAudience = jwtBearer.Audience,
                ValidIssuer = jwtBearer.Issuer,
                IssuerSigningKey = securityKey,
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
            };

            _options.TryAdd(jwtBearer.Name, new JwtBearerOptions
            {
                TokenValidationParameters = tokenValidationParameters,
                RequireHttpsMetadata = false
            });
            _authorizationOptions.AddPolicy(jwtBearer.Name, policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddAuthenticationSchemes(jwtBearer.Name);
            });
        }

        return Task.CompletedTask;
    }
}