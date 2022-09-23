using k8s;
using k8s.Models;
using Newtonsoft.Json;

namespace Sail.Kubernetes.Controller.Models;

[KubernetesEntity(ApiVersion = KubeApiVersion, Group = KubeGroup, Kind = KubeKind, PluralName = "httproutes")]
public class V1beta1HttpRoute : IKubernetesObject<V1ObjectMeta>, ISpec<V1beta1HttpRouteSpec>
{
    public const string KubeApiVersion = "v1beta1";
    public const string KubeGroup = "gateway.networking.k8s.io";
    public const string KubeKind = "HTTPRoute";

    [JsonProperty("apiVersion")] public string ApiVersion { get; set; }
    [JsonProperty("kind")] public string Kind { get; set; }
    [JsonProperty("metadata")] public V1ObjectMeta Metadata { get; set; }
    [JsonProperty("spec")] public V1beta1HttpRouteSpec Spec { get; set; }
}

public class V1beta1HttpRouteSpec
{
    [JsonProperty("parentRefs")] public List<V1beta1HttpRouteParentRef> ParentRefs { get; set; }
    [JsonProperty("hostnames")] public List<string> Hostnames { get; set; }
    [JsonProperty("rules")] public List<V1beta1HttpRouteRule> Rules { get; set; }
}

public class V1beta1HttpRouteParentRef
{
    [JsonProperty("group")] public string Group { get; set; }
    [JsonProperty("kind")] public string Kind { get; set; }
    [JsonProperty("namespace")] public string Namespace { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("sectionName")] public string SectionName { get; set; }
    [JsonProperty("port")] public int PortNumber { get; set; }
}

public class V1beta1HttpRouteRule
{
    [JsonProperty("matches")] public List<V1beta1HttpRouteMatch> Matches { get; set; }

    [JsonProperty("backendRefs")] public List<V1beta1HttpBackendRef> BackendRefs { get; set; }
}

public class V1beta1HttpRouteMatch
{

    [JsonProperty("path")] public V1beta1HttpPathMatch Path { get; set; }

    [JsonProperty("headers")] public List<V1beta1HttpHeaderMatch> Headers { get; set; }

    [JsonProperty("queryParams")] public V1beta1HttpQueryParamMatch QueryParams { get; set; }

    [JsonProperty("method")] public string Method { get; set; }

}

public class V1beta1HttpPathMatch
{
    [JsonProperty("type")] public string Type { get; set; }

    [JsonProperty("value")] public string Value { get; set; }
}

public class V1beta1HttpHeaderMatch
{
    [JsonProperty("type")] public string Type { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("value")] public string Value { get; set; }
}

public class V1beta1HttpQueryParamMatch
{
    [JsonProperty("type")] public string Type { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("value")] public string Value { get; set; }
}

public class V1beta1HttpBackendRef
{
    [JsonProperty("group")] public string Group { get; set; }
    [JsonProperty("kind")] public string Kind { get; set; }
    [JsonProperty("name")] public string Name { get; set; }
    [JsonProperty("namespace")] public string Namespace { get; set; }
    [JsonProperty("port")] public int? Port { get; set; }
    [JsonProperty("weight")] public int Weight { get; set; }
}