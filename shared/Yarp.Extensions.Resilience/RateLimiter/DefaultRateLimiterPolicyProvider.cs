using System.Collections.Concurrent;

namespace Yarp.Extensions.Resilience.RateLimiter;

public class DefaultRateLimiterPolicyProvider : IRateLimiterPolicyProvider
{
    private readonly ConcurrentDictionary<string, RateLimiterPolicy> _policies = new();
    
    public RateLimiterPolicy? GetPolicy(string key)
    {
        return _policies.GetValueOrDefault(key);
    }
}