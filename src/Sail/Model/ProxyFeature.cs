using Sail.Configuration;

namespace Sail.Model;

public class ProxyFeature : IProxyFeature
{
    public CanaryModel Canary { get; init; } = default!;
    public CircuitBreakerModel CircuitBreaker { get; init; } = default!;
    public RateLimiterModel RateLimiter { get; init; } = default!;
}