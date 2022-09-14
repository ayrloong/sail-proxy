using k8s.Models;
using Sail.Kubernetes.Gateway.Models;
using YamlDotNet.Serialization;

namespace Sail.Kubernetes.Gateway.Converters;

public static class SailParser
{
    private static readonly Deserializer YamlDeserializer = new();

    internal static void ConvertFromKubernetesGateway(SailGatewayContext gatewayContext,
        SailConfigContext configContext)
    {
        var spec = gatewayContext.HttpRoute.Spec;

        foreach (var rule in spec?.Rules ?? Enumerable.Empty<V1beta1HttpRouteRule>())
        {
            HandleGatewayRule(gatewayContext, configContext, rule);
        }
    }
    private static void HandleGatewayRule(
        SailGatewayContext gatewayContext,
        SailConfigContext configContext,
        V1beta1HttpRouteRule rule)
    {
        foreach (var backend in rule.BackendRefs ?? Enumerable.Empty<V1beta1HttpBackendRef>())
        {
            var service = gatewayContext.Services.SingleOrDefault(s => s.Metadata.Name == backend.Name);
            var servicePort = service.Spec.Ports.SingleOrDefault(p => p.Port == backend.Port);
            HandleGatewayRulePath(gatewayContext, configContext, servicePort, rule, backend);
        }
    }

    private static void HandleGatewayRulePath(
        SailGatewayContext gatewayContext,
        SailConfigContext configContext,
        V1ServicePort servicePort,
        V1beta1HttpRouteRule rule,
        V1beta1HttpBackendRef backend)
    {
        var clusters = configContext.ClusterTransfers;
        var routes = configContext.Routes;

        var key = UpstreamName(gatewayContext.Gateway.Metadata.NamespaceProperty, backend);
        var cluster = clusters[key];
        cluster.ClusterId = key;
        cluster.LoadBalancingPolicy = gatewayContext.Options.LoadBalancingPolicy;
        cluster.SessionAffinity = gatewayContext.Options.SessionAffinity;
        cluster.HealthCheck = gatewayContext.Options.HealthCheck;
        cluster.HttpClientConfig = gatewayContext.Options.HttpClientConfig;

    }
    private static string UpstreamName(string namespaceName, V1beta1HttpBackendRef backend)
    {
        if (backend is not null)
        {
            return $"{backend.Name}.{namespaceName}:{backend.Port}";

        }
        return $"{namespaceName}-INVALID";
    }
}