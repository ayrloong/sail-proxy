using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Sail.Kubernetes.Protocol.Configuration;

namespace Sail.Kubernetes.Protocol.Middlewares;

public class JwtBearerMiddleware : IMiddleware
{
    private readonly IAuthenticationSchemeProvider _provider;
    private readonly IOptionsMonitorCache<JwtBearerOptions> _options;
    private readonly AuthorizationOptions _authorizationOptions;

    public JwtBearerMiddleware(
        IAuthenticationSchemeProvider provider,
        IOptionsMonitorCache<JwtBearerOptions> options,
        IOptions<AuthorizationOptions> authorizationOptions)
    {
        _provider = provider;
        _options = options;
        _authorizationOptions = authorizationOptions.Value;
    }

    public async Task ApplyAsync(IEnumerable<MiddlewareConfig> middlewares)
    {
        _options.Clear();
        var jwtBearers = middlewares.Where(x => x.JwtBearer is not null).Select(x => x.JwtBearer);
        foreach (var jwtBearer in jwtBearers)
        {
            var scheme =
                new AuthenticationScheme(jwtBearer.Name, displayName: null, typeof(JwtBearerHandler));
            _provider.TryAddScheme(scheme);

            var openidConfigManaged = new ConfigurationManager<OpenIdConnectConfiguration>(
                jwtBearer.OpenIdConfiguration,
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever());

            var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtBearer.Secret));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidAudience = jwtBearer.Audience,
                ValidIssuer = jwtBearer.Issuer,
                IssuerSigningKey = securityKey,
                ValidateIssuer = false,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
            };

            _options.TryAdd(jwtBearer.Name, new JwtBearerOptions
            {
                Audience = jwtBearer.Audience,
                Authority = jwtBearer.Issuer,
                RequireHttpsMetadata = false,
                TokenValidationParameters = tokenValidationParameters,
                ConfigurationManager = openidConfigManaged,
                MetadataAddress = jwtBearer.OpenIdConfiguration
            });
            _authorizationOptions.AddPolicy(jwtBearer.Name,
                policy => { policy.RequireAuthenticatedUser().AddAuthenticationSchemes(jwtBearer.Name); });
        }
    }
}