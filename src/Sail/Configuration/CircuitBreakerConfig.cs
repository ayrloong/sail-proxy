namespace Sail.Configuration;

public class CircuitBreakerConfig
{
    public string RouteId { get; set; }
    public bool Enabled { get; set; }
    public TimeSpan Interval { get; set; }
    public TimeSpan Timeout { get; set; }
}