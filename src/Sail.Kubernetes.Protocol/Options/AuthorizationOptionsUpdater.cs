using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Sail.Kubernetes.Protocol.Options;

public class AuthorizationOptionsUpdater : IAuthorizationOptionsUpdater
{
    private readonly AuthorizationOptions _options;

    public AuthorizationOptionsUpdater(IOptions<AuthorizationOptions> options)
    {
        _options = options.Value;
    }

    public Task UpdateAsync(string name)
    {
        _options.AddPolicy(name, policy => { policy.RequireAuthenticatedUser(); });
        return Task.CompletedTask;
    }
}