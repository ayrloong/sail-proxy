using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace ApiSample;

public class DynamicRateLimitConfigurationProvider
{
    private RateLimiterOptions options = new();

    public void AddRateLimitOptions()
    {
        options.AddTokenBucketLimiter("test", options =>
        {
            options.TokenLimit = 1;
            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            options.QueueLimit = 1;
            options.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
            options.TokensPerPeriod = 1;
        });
    }
    public RateLimiterOptions GetRateLimitOptions()
    {
        return options;
    }
}