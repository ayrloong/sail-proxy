namespace Yarp.Extensions.Resilience;

public class ResiliencePipelineConfig
{
    public string Name { get; set; }
    public RetryConfig? Retry { get; set; }
    public CircuitBreakerConfig? CircuitBreaker { get; set; }
}