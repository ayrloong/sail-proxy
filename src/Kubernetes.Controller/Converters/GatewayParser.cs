using k8s.Models;
using Sail.Kubernetes.Controller.Caching;
using Sail.Kubernetes.Controller.Models;
using Yarp.ReverseProxy.Configuration;

namespace Sail.Kubernetes.Controller.Converters;

public static class GatewayParser
{
    internal static void ConvertFromKubernetesGateway(GatewayContext gatewayContext, ConfigContext configContext)
    {
        var routes = gatewayContext.HttpRoutes;
        foreach (var route in routes)
        {

            HandleGatewayRoute(gatewayContext, route, route.Spec.Hostnames, configContext);
        }
    }

    private static void HandleGatewayRoute(GatewayContext gatewayContext,
        HttpRouteData httpRoute,
        IReadOnlyList<string> hosts,
        ConfigContext configContext)
    {
        var rules = httpRoute.Spec.Rules;
        foreach (var rule in rules)
        {
            HandleGatewayRule(gatewayContext, gatewayContext.Endpoints, rule, hosts, httpRoute, configContext);
        }
    }

    private static void HandleGatewayRule(GatewayContext gatewayContext,
        List<Endpoints> endpoints,
        V1beta1HttpRouteRule rule,
        IReadOnlyList<string> hosts,
        HttpRouteData httpRoute,
        ConfigContext configContext)
    {
        var paths = rule.Matches.Where(x => x.Path != null).Select(x => x.Path);
        foreach (var path in paths)
        {
            HandleGatewayRulePath(gatewayContext, endpoints, path, rule, hosts, httpRoute, configContext);
        }
    }

    private static void HandleGatewayRulePath(GatewayContext gatewayContext,
        List<Endpoints> endpoints,
        V1beta1HttpPathMatch path,
        V1beta1HttpRouteRule rule,
        IReadOnlyList<string> hosts,
        HttpRouteData httpRoute,
        ConfigContext configContext)
    {

        var routes = configContext.Routes;
        var route = new RouteConfig
        {
            Match = new RouteMatch()
            {
                Hosts = hosts,
                Path = path.Value,
            },
            RouteId = $"{httpRoute.Metadata.Name}.{httpRoute.Metadata.NamespaceProperty}:{path.Value}",
        };
        var weightCluster = new WeightClusterConfig
        {
            Clusters = new List<WeightCluster>()
        };

        foreach (var backendRef in rule.BackendRefs)
        {
            var service = gatewayContext.Services.SingleOrDefault(s => s.Metadata.Name == backendRef.Name);
            var servicePort = service.Spec?.Ports.SingleOrDefault(p => MatchesPort(p, backendRef));
            if (servicePort != null)
            {
                var key = UpstreamName(httpRoute.Metadata.NamespaceProperty, backendRef);
                HandleGatewayRuleBackend(gatewayContext, servicePort, endpoints, backendRef, key, configContext);
                route.ClusterId = key;
                weightCluster.Clusters.Add(new WeightCluster
                {
                    ClusterId = key,
                    Weight = backendRef.Weight,
                });
            }
        }

        if (rule.BackendRefs.Count > 1)
        {
            route.WeightCluster = weightCluster;
        }

        routes.Add(route);
    }

    private static void HandleGatewayRuleBackend(GatewayContext gatewayContext,
        V1ServicePort servicePort,
        List<Endpoints> endpoints,
        V1beta1HttpBackendRef backendRef,
        string key,
        ConfigContext configContext)
    {
        var clusters = configContext.ClusterTransfers;
        if (!clusters.ContainsKey(key))
        {
            clusters.Add(key, new ClusterTransfer());
        }

        var cluster = clusters[key];
        cluster.ClusterId = key;

        var subsets = endpoints.SingleOrDefault(x => x.Name == backendRef?.Name).Subsets;
        foreach (var subset in subsets ?? Enumerable.Empty<V1EndpointSubset>())
        {
            foreach (var port in subset.Ports ?? Enumerable.Empty<Corev1EndpointPort>())
            {
                if (!MatchesPort(port, servicePort?.TargetPort))
                {
                    continue;
                }

                foreach (var address in subset.Addresses ?? Enumerable.Empty<V1EndpointAddress>())
                {
                    var protocol = gatewayContext.Options.Https ? "https" : "http";
                    var uri = $"{protocol}://{address.Ip}:{port.Port}";
                    cluster.Destinations[uri] = new DestinationConfig()
                    {
                        Address = uri
                    };
                }
            }
        }
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

    private static string UpstreamName(string namespaceName, V1beta1HttpBackendRef backendRef)
    {
        if (backendRef is not null)
        {
            if (backendRef.Port is > 0)
            {
                return $"{backendRef.Name}.{namespaceName}:{backendRef.Port}";
            }

            if (!string.IsNullOrWhiteSpace(backendRef.Name))
            {
                return $"{backendRef.Name}.{namespaceName}:{backendRef.Name}";
            }
        }

        return $"{namespaceName}-INVALID";
    }

    private static bool MatchesPort(V1ServicePort port1, V1beta1HttpBackendRef port2)
    {
        if (port1 is null || port2 is null)
        {
            return false;
        }

        if (port2.Port is not null && port2.Port == port1.Port)
        {
            return true;
        }

        return port2.Name is not null && string.Equals(port2.Name, port1.Name, StringComparison.Ordinal);
    }

    private static List<RouteHeader> ConvertHeaderMatch(IEnumerable<V1beta1HttpHeaderMatch> headerMatches)
    {
        return new List<RouteHeader>();
    }

    private static List<RouteQueryParameter> ConvertQueryParamMatch(
        IEnumerable<V1beta1HttpQueryParamMatch> queryParamMatches)
    {
        return new List<RouteQueryParameter>();
    }
}