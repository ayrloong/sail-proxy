namespace Yarp.Extensions.Resilience.RateLimiter;

public class RateLimiterPolicy
{
    public int PermitLimit { get; set; }
    public TimeSpan Window { get; set; }
}