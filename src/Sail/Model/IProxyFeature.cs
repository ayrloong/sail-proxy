namespace Sail.Model;

public interface IProxyFeature
{
    public CanaryModel Canary { get; init; }
    public CircuitBreakerModel CircuitBreaker { get; init; }
    public RateLimiterModel RateLimiter { get; init; }
}