using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Forwarder;

namespace Sail.Kubernetes.Controller.Converters;

internal sealed class ClusterTransfer
{
    public Dictionary<string, DestinationConfig> Destinations { get; set; } = new();
    public string ClusterId { get; set; }
    public string LoadBalancingPolicy { get; set; }
    public SessionAffinityConfig SessionAffinity { get; set; }
    public HealthCheckConfig HealthCheck { get; set; }
    public HttpClientConfig HttpClientConfig { get; set; }
    public ForwarderRequestConfig RequestConfig { get; set; }
}