namespace Yarp.Extensions.Resilience.RateLimiter;

public interface IRateLimiterPolicyProvider
{
    RateLimiterPolicy? GetPolicy(string key);
}