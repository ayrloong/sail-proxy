using k8s.Models;
using Sail.Kubernetes.Controller.Caching;
using YamlDotNet.Serialization;
using Yarp.ReverseProxy.Configuration;

namespace Sail.Kubernetes.Controller.Converters;

internal static class SailParser
{
    private static readonly Deserializer YamlDeserializer = new();

    internal static void ConvertFromKubernetesIngress(SailIngressContext ingressContext,
        SailConfigContext configContext)
    {
        var spec = ingressContext.Ingress.Spec;
        var defaultBackend = spec?.DefaultBackend;
        var defaultService = defaultBackend?.Service;
        IList<V1EndpointSubset> defaultSubsets = default;
        if (!string.IsNullOrEmpty(defaultService?.Name))
        {
            defaultSubsets = ingressContext.Endpoints.SingleOrDefault(x => x.Name == defaultService?.Name).Subsets;
        }

        var options = HandleAnnotations(ingressContext, ingressContext.Ingress.Metadata);

        foreach (var rule in spec.Rules ?? Enumerable.Empty<V1IngressRule>())
        {
            HandleIngressRule(ingressContext, ingressContext.Endpoints, defaultSubsets, rule, configContext);
        }
    }

    private static void HandleIngressRule(SailIngressContext ingressContext, List<Endpoints> endpoints,
        IList<V1EndpointSubset> defaultSubsets,
        V1IngressRule rule, SailConfigContext configContext)
    {
        var http = rule.Http;
        foreach (var path in http.Paths ?? Enumerable.Empty<V1HTTPIngressPath>())
        {
            var service = ingressContext.Services.SingleOrDefault(s => s.Metadata.Name == path.Backend.Service.Name);
            var servicePort = service.Spec.Ports.SingleOrDefault(p => MatchesPort(p, path.Backend.Service.Port));
            HandleIngressRulePath(ingressContext, servicePort, endpoints, defaultSubsets, rule, path, configContext);
        }
    }

    private static void HandleIngressRulePath(SailIngressContext ingressContext, V1ServicePort servicePort,
        List<Endpoints> endpoints, IList<V1EndpointSubset> defaultSubsets, V1IngressRule rule, V1HTTPIngressPath path,
        SailConfigContext configContext)
    {
        var backend = path.Backend;
        var ingressServiceBackend = backend.Service;
        var subsets = defaultSubsets;

        var clusters = configContext.ClusterTransfers;
        var routes = configContext.Routes;
        if (!string.IsNullOrEmpty(ingressServiceBackend.Name))
        {
            subsets = endpoints.SingleOrDefault(x => x.Name == ingressServiceBackend.Name).Subsets;
        }

        var key = UpstreamName(ingressContext.Ingress.Metadata.NamespaceProperty, ingressServiceBackend);
        if (!clusters.ContainsKey(key))
        {
            clusters.Add(key, new ClusterTransfer());
        }

        var cluster = clusters[key];
        cluster.ClusterId = key;
        cluster.LoadBalancingPolicy = ingressContext.Options.LoadBalancingPolicy;
        cluster.SessionAffinity = ingressContext.Options.SessionAffinity;
        cluster.HealthCheck = ingressContext.Options.HealthCheck;
        cluster.HttpClientConfig = ingressContext.Options.HttpClientConfig;
        foreach (var subset in subsets)
        {
            foreach (var port in subset.Ports)
            {
                if (!MatchesPort(port, servicePort.TargetPort))
                {
                    continue;
                }

                var pathMatch = FixupPathMatch(path);
                var host = rule.Host;

                routes.Add(new RouteConfig
                {
                    Match = new RouteMatch
                    {
                        Hosts = host is not null ? new[] { host } : Array.Empty<string>(),
                        Path = pathMatch
                    },
                    ClusterId = cluster.ClusterId,
                    RouteId =
                        $"{ingressContext.Ingress.Metadata.Name}.{ingressContext.Ingress.Metadata.NamespaceProperty}:{path.Path}",
                    Transforms = ingressContext.Options.Transforms,
                    MaxRequestBodySize = ingressContext.Options.MaxRequestBodySize ?? -1,
                    AuthorizationPolicy = ingressContext.Options.AuthorizationPolicy,
                    RateLimiterPolicy = ingressContext.Options.RateLimiterPolicy,
                    CorsPolicy = ingressContext.Options.CorsPolicy,
                    Metadata = ingressContext.Options.RouteMetadata
                });

                foreach (var address in subset.Addresses)
                {
                    var protocol = ingressContext.Options.Https ? "https" : "http";
                    var uri = $"{protocol}://{address.Ip}:{port.Port}";
                    cluster.Destinations[uri] = new DestinationConfig
                    {
                        Address = uri
                    };
                }
            }
        }
    }

    private static SailIngressOptions HandleAnnotations(SailIngressContext ingressContext, V1ObjectMeta metadata)
    {
        var options = ingressContext.Options;
        var annotations = metadata.Annotations;
        if (annotations is null)
        {
            return options;
        }

        if (annotations.TryGetValue("sail.ingress.kubernetes.io/plugins", out var plugins))
        {
            foreach (var middleware in YamlDeserializer.Deserialize<List<string>>(plugins))
            {
                HandlePlugin(ingressContext, middleware);
            }
        }

        return options;
    }

    private static void HandlePlugin(SailIngressContext ingressContext, string pluginName)
    {
        var plugin = ingressContext.Plugins.SingleOrDefault(s => s.Metadata.Name == pluginName);

        var spec = plugin.Spec;

        if (spec?.AddPrefix is not null)
        {
            var addPrefix = spec.AddPrefix;
            ingressContext.Options.Transforms.Add(new Dictionary<string, string>
            {
                {
                    "PathPrefix", addPrefix.Prefix
                }
            });
        }

        if (spec?.RemovePrefix is not null)
        {
            var removePrefix = spec.RemovePrefix;
            foreach (var prefix in removePrefix.Prefixes)
            {
                ingressContext.Options.Transforms.Add(new Dictionary<string, string>
                {
                    {
                        "PathRemovePrefix", prefix
                    }
                });
            }
        }

        if (spec?.Limits is not null)
        {
            var limits = spec.Limits;
            ingressContext.Options.MaxRequestBodySize = limits.MaxRequestBodySize;
        }

        if (spec?.JwtBearer is not null)
        {
            ingressContext.Options.AuthorizationPolicy = plugin.Metadata.Name;
        }

        if (spec?.Cors is not null)
        {
            ingressContext.Options.CorsPolicy = plugin.Metadata.Name;
        }

        if (spec?.RateLimiter is not null)
        {
            ingressContext.Options.RateLimiterPolicy = plugin.Metadata.Name;
        }
    }

    private static string UpstreamName(string namespaceName, V1IngressServiceBackend ingressServiceBackend)
    {
        if (ingressServiceBackend is not null)
        {
            if (ingressServiceBackend.Port.Number is > 0)
            {
                return $"{ingressServiceBackend.Name}.{namespaceName}:{ingressServiceBackend.Port.Number}";
            }

            if (!string.IsNullOrWhiteSpace(ingressServiceBackend.Port.Name))
            {
                return $"{ingressServiceBackend.Name}.{namespaceName}:{ingressServiceBackend.Port.Name}";
            }
        }

        return $"{namespaceName}-INVALID";
    }

    private static string FixupPathMatch(V1HTTPIngressPath path)
    {
        var pathMatch = path.Path;

        if (string.Equals(path.PathType, "Prefix", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(path.PathType, "ImplementationSpecific", StringComparison.OrdinalIgnoreCase))
        {
            if (!pathMatch.EndsWith("/", StringComparison.Ordinal))
            {
                pathMatch += "/";
            }

            pathMatch += "{**catch-all}";
        }

        return pathMatch;
    }

    private static bool MatchesPort(Corev1EndpointPort port1, IntstrIntOrString port2)
    {
        if (port1 is null || port2 is null)
        {
            return false;
        }

        if (int.TryParse(port2, out var port2Number) && port2Number == port1.Port)
        {
            return true;
        }

        return string.Equals(port2, port1.Name, StringComparison.OrdinalIgnoreCase);
    }

    private static bool MatchesPort(V1ServicePort port1, V1ServiceBackendPort port2)
    {
        if (port1 is null || port2 is null)
        {
            return false;
        }

        if (port2.Number is not null && port2.Number == port1.Port)
        {
            return true;
        }

        return port2.Name is not null && string.Equals(port2.Name, port1.Name, StringComparison.Ordinal);
    }
}