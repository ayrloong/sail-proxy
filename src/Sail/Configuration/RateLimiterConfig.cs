namespace Sail.Configuration;

public class RateLimiterConfig
{
    public string RouteId { get; set; }
    public bool? Enabled { get; init; }
}