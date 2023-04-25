using System.Text.Json.Serialization;
using Sail.Kubernetes.Protocol.Configuration;
using Yarp.ReverseProxy.Configuration;

namespace Sail.Kubernetes.Protocol;

public enum MessageType
{
    Heartbeat,
    Update,
    Remove,
}

public struct Message
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public MessageType MessageType { get; set; }

    public string Key { get; set; }
    public List<RouteConfig> Routes { get; set; }
    public List<ClusterConfig> Cluster { get; set; }
    public List<TlsConfig> Tls { get; set; }
    public List<PluginConfig> Plugins { get; set; }
}