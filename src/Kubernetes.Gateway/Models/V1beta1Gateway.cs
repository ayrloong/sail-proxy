using k8s;
using k8s.Models;
using Newtonsoft.Json;

namespace Sail.Kubernetes.Gateway.Models;

[KubernetesEntity(ApiVersion = KubeApiVersion, Group = KubeGroup, Kind = KubeKind, PluralName = "gateways")]
public class V1beta1Gateway : IKubernetesObject<V1ObjectMeta>, ISpec<V1beta1GatewaySpec>
{
    public const string KubeApiVersion = "v1beta1";
    public const string KubeGroup = "gateway.networking.k8s.io";
    public const string KubeKind = "Gateway";

    [JsonProperty("apiVersion")] public string ApiVersion { get; set; }
    [JsonProperty("kind")] public string Kind { get; set; }
    [JsonProperty("metadata")] public V1ObjectMeta Metadata { get; set; }
    [JsonProperty("spec")] public V1beta1GatewaySpec Spec { get; set; }
}

public class V1beta1GatewaySpec
{
    [JsonProperty(PropertyName = "gatewayClassName")]
    public string GatewayClassName { get; set; }

    [JsonProperty(PropertyName = "listeners")]
    public List<V1beta1GatewayListener> Listeners { get; set; }

    [JsonProperty(PropertyName = "addresses")]
    public List<V1beta1GatewayAddress> Addresses { get; set; }
}
public class V1beta1GatewayListener
{
    [JsonProperty("name")]
    public string Name { get; set; }
    [JsonProperty("hostname")]
    public string Hostname { get; set; }
    [JsonProperty("port")]
    public int Port { get; set; }
    [JsonProperty("protocol")]
    public string Protocol { get; set; }
}

public class V1beta1GatewayAddress
{
    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; }
    [JsonProperty(PropertyName = "value")]
    public string Value { get; set; }
}