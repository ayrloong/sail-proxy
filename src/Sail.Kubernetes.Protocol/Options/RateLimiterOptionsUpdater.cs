using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace Sail.Kubernetes.Protocol.Options;

public class RateLimiterOptionsUpdater : IRateLimiterOptionsUpdater
{
    private readonly RateLimiterOptions _options;

    public RateLimiterOptionsUpdater(IOptions<RateLimiterOptions> options)
    {
        _options = options.Value;
    }

    public Task UpdateAsync(string name)
    {
        _options.AddFixedWindowLimiter(name, options =>
        {
            options.PermitLimit = 4;
            options.Window = TimeSpan.FromSeconds(12);
            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            options.QueueLimit = 2;
        });
        return Task.CompletedTask;
    }
}