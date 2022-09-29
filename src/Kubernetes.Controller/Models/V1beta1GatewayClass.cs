using System.Text.Json.Serialization;
using k8s;
using k8s.Models;
using Newtonsoft.Json;

namespace Sail.Kubernetes.Controller.Models;

[KubernetesEntity(ApiVersion = KubeApiVersion, Group = KubeGroup, Kind = KubeKind, PluralName = "gatewayclasses")]
public class V1beta1GatewayClass : IKubernetesObject<V1ObjectMeta>, ISpec<V1beta1GatewayClassSpec>,
    IStatus<V1beta1GatewayClassStatus>
{
    public const string KubeApiVersion = "v1beta1";
    public const string KubeGroup = "gateway.networking.k8s.io";
    public const string KubeKind = "GatewayClass";

    [JsonProperty("apiVersion")] public string ApiVersion { get; set; }
    [JsonProperty("kind")] public string Kind { get; set; }
    [JsonProperty("metadata")] public V1ObjectMeta Metadata { get; set; }
    [JsonProperty("spec")] public V1beta1GatewayClassSpec Spec { get; set; }
    [JsonProperty("status")] public V1beta1GatewayClassStatus Status { get; set; }
}


public class V1beta1GatewayClassSpec
{
    [JsonProperty(PropertyName = "controllerName")]
    public string ControllerName { get; set; }
}

public class V1beta1GatewayClassStatus
{
    [JsonProperty(PropertyName = "conditions")]
    public List<V1beta1GatewayClassStatusCondition> Conditions { get; set; }
}

public class V1beta1GatewayClassStatusCondition
{
    [JsonProperty(PropertyName = "lastTransitionTime")]
    public DateTime LastTransitionTime { get; set; }

    [JsonProperty(PropertyName = "message")]
    public string Message { get; set; }

    [JsonProperty(PropertyName = "reason")]
    public string Reason { get; set; }

    [JsonProperty(PropertyName = "status")]
    public string Status { get; set; }

    [JsonProperty(PropertyName = "type")] public string Type { get; set; }
}