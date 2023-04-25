using Yarp.ReverseProxy.Configuration;

namespace Sail.Kubernetes.Controller.Converters;

internal sealed class SailIngressOptions
{
    public bool Https { get; set; }
    public List<Dictionary<string, string>> Transforms { get; set; } = new();
    public long? MaxRequestBodySize { get; set; }
    public string AuthorizationPolicy { get; set; }
    public string RateLimiterPolicy { get; set; }
    public SessionAffinityConfig SessionAffinity { get; set; }
    public HttpClientConfig HttpClientConfig { get; set; }
    public string LoadBalancingPolicy { get; set; }
    public string CorsPolicy { get; set; }
    public HealthCheckConfig HealthCheck { get; set; }
    public Dictionary<string, string> RouteMetadata { get; set; }
}