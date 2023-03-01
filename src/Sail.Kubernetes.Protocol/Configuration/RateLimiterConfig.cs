namespace Sail.Kubernetes.Protocol.Configuration;

public class RateLimiterConfig
{
    public string Name { get; set; }
    public int PermitLimit { get; set; }
    public int Window { get; set; }
    public int QueueLimit { get; set; }
}