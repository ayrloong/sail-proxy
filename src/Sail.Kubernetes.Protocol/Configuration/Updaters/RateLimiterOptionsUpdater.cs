using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace Sail.Kubernetes.Protocol.Configuration;

public class RateLimiterOptionsUpdater : IRateLimiterOptionsUpdater
{
    private readonly RateLimiterOptions _options;

    public RateLimiterOptionsUpdater(IOptions<RateLimiterOptions> options)
    {
        _options = options.Value;
    }

    public Task UpdateAsync(RateLimiterConfig rateLimiter)
    {
        _options.AddFixedWindowLimiter(rateLimiter.Name, options =>
        {
            options.PermitLimit = rateLimiter.PermitLimit;
            options.Window = TimeSpan.FromSeconds(rateLimiter.Window);
            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            options.QueueLimit = rateLimiter.QueueLimit;
        });
        return Task.CompletedTask;
    }
}