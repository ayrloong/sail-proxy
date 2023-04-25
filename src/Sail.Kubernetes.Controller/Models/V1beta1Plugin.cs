using System.Text.Json.Serialization;
using k8s;
using k8s.Models;

namespace Sail.Kubernetes.Controller.Models;

[KubernetesEntity(ApiVersion = KubeApiVersion, Group = KubeGroup, Kind = KubeKind, PluralName = "plugins")]
public class V1beta1Plugin: IKubernetesObject<V1ObjectMeta>, ISpec<V1beta1PluginSpec>
{
    public const string KubeApiVersion = "v1beta1";
    public const string KubeGroup = "configuration.inendless.io";
    public const string KubeKind = "Plugin";
    
    [JsonPropertyName("apiVersion")] public string ApiVersion { get; set; }
    [JsonPropertyName("kind")] public string Kind { get; set; }
    [JsonPropertyName("metadata")] public V1ObjectMeta Metadata { get; set; }
    [JsonPropertyName("spec")] public V1beta1PluginSpec Spec { get; set; }
}

public class V1beta1PluginSpec
{
    [JsonPropertyName("removePrefix")] public RemovePrefix RemovePrefix { get; set; }
    [JsonPropertyName("addPrefix")] public AddPrefix AddPrefix { get; set; }
    [JsonPropertyName("jwtBearer")] public JwtBearer JwtBearer { get; set; }
    [JsonPropertyName("cors")] public Cors Cors { get; set; }
    [JsonPropertyName("rateLimiter")] public RateLimiter RateLimiter { get; set; }
    [JsonPropertyName("limits")] public Limits Limits { get; set; }
}

public class RemovePrefix
{
    [JsonPropertyName("prefixes")] public List<string> Prefixes { get; set; }
}

public class AddPrefix
{
    [JsonPropertyName("prefix")] public string Prefix { get; set; }
}

public class JwtBearer
{
    [JsonPropertyName("secret")] public string Secret { get; set; }
    [JsonPropertyName("issuer")] public string Issuer { get; set; }
    [JsonPropertyName("audience")] public string Audience { get; set; }

    [JsonPropertyName("openIdConfiguration")]
    public string OpenIdConfiguration { get; set; }
}

public class Cors
{
    [JsonPropertyName("allowOrigins")] public List<string> AllowOrigins { get; set; }
    [JsonPropertyName("allowMethods")] public List<string> AllowMethods { get; set; }
    [JsonPropertyName("allowHeaders")] public List<string> AllowHeaders { get; set; }
}

public class RateLimiter
{
    [JsonPropertyName("permitLimit")] public int PermitLimit { get; set; }
    [JsonPropertyName("window")] public int Window { get; set; }
    [JsonPropertyName("queueLimit")] public int QueueLimit { get; set; }
}

public class Limits
{
    [JsonPropertyName("maxRequestBodySize")]
    public long MaxRequestBodySize { get; set; }
}