using k8s;
using k8s.Models;
using Newtonsoft.Json;

namespace Sail.Kubernetes.Controller.Models;

[KubernetesEntity(ApiVersion = KubeApiVersion, Group = KubeGroup, Kind = KubeKind, PluralName = "gatewayclasses")]
public class V1beta1GatewayClass : IKubernetesObject<V1ObjectMeta>, ISpec<V1beta1GatewayClassSpec>
{
    public const string KubeApiVersion = "v1beta1";
    public const string KubeGroup = "gateway.networking.k8s.io";
    public const string KubeKind = "GatewayClass";

    [JsonProperty("apiVersion")] public string ApiVersion { get; set; }
    [JsonProperty("kind")] public string Kind { get; set; }
    [JsonProperty("metadata")] public V1ObjectMeta Metadata { get; set; }
    [JsonProperty("spec")] public V1beta1GatewayClassSpec Spec { get; set; }
}
public class V1beta1GatewayClassSpec
{
    [JsonProperty(PropertyName = "controllerName")]
    public string ControllerName { get; set; }
}