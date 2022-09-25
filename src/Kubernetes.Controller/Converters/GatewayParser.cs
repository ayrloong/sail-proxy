using k8s.Models;
using Sail.Kubernetes.Controller.Caching;
using Sail.Kubernetes.Controller.Models;
using Yarp.ReverseProxy.Configuration;

namespace Sail.Kubernetes.Controller.Converters;

public static class GatewayParser
{
    internal static void ConvertFromKubernetesGateway(GatewayContext gatewayContext,ConfigContext configContext)
    {
        var routes = gatewayContext.HttpRoutes;
        foreach (var route in routes)
        {
            HandleGatewayRoute(gatewayContext, route, route.Spec.Hostnames, configContext);
        }
    }
    
    private static void HandleGatewayRoute(GatewayContext gatewayContext, 
        HttpRouteData route,
        IReadOnlyList<string> hostnames,
        ConfigContext configContext)
    {
        var rules = route.Spec.Rules;
        foreach (var rule in rules)
        {
            HandleGatewayRule(gatewayContext, rule, hostnames, configContext);
        }
    }
    
    private static void HandleGatewayRule(GatewayContext gatewayContext, 
        V1beta1HttpRouteRule rule,
        IReadOnlyList<string> hostnames,
        ConfigContext configContext)
    {

        var pathMatches = rule.Matches.Where(x => x.Path != null).Select(x => x.Path).ToList();
        var methodMatches= rule.Matches.Where(x => x.Method != null).Select(x => x.Method).ToList();
        var queryParamMatches = rule.Matches.Where(x => x.QueryParams != null).Select(x => x.QueryParams).ToList();
        var headerMatches = rule.Matches.Where(x => x.Headers != null).SelectMany(x => x.Headers).ToList();

        var clusterKey = "test";
        
        foreach (var pathMatch in pathMatches)
        {
            HandleGatewayRulePath(gatewayContext, pathMatch, hostnames, methodMatches, queryParamMatches, headerMatches,
                clusterKey,
                configContext);
        }

        HandleGatewayRuleBackend(gatewayContext,rule.BackendRefs,clusterKey,configContext);

    }

    private static void HandleGatewayRulePath(GatewayContext gatewayContext,
        V1beta1HttpPathMatch path,
        IReadOnlyList<string> hostnames,
        IReadOnlyList<string> methodMatches,
        List<V1beta1HttpQueryParamMatch> queryParamMatches,
        List<V1beta1HttpHeaderMatch> headerMatches,
        string clusterKey,
        ConfigContext configContext)
    {
       
        var routes = configContext.Routes;
        var headers = ConvertHeaderMatch(headerMatches);
        var queryParameters = ConvertQueryParamMatch(queryParamMatches);

        routes.Add(new RouteConfig()
        {
            ClusterId = clusterKey,
            Match = new RouteMatch()
            {
                Hosts = hostnames,
                Path = path.Value,
                Methods = methodMatches,
                Headers = headers,
                QueryParameters = queryParameters
            }
        });
    }

    private static void HandleGatewayRuleBackend(GatewayContext gatewayContext,
        List<V1beta1HttpBackendRef> backendRefs,
        string clusterKey,
        ConfigContext configContext)
    {
        var clusters = configContext.ClusterTransfers;
        if (!clusters.ContainsKey(clusterKey))
        {
            clusters.Add(clusterKey, new ClusterTransfer());
        }
        
        var cluster = clusters[clusterKey];
        cluster.ClusterId = clusterKey;
        foreach (var backendRef in backendRefs)
        {
            var protocol = gatewayContext.Options.Https ? "https" : "http";
            //var service = gatewayContext.Services.SingleOrDefault(s => s.Metadata.Name == backendRef.Name);
            //var servicePort = service.Spec.Ports.SingleOrDefault(p => MatchesPort(p, backendRef));
            //var uri = $"{protocol}://{service.Spec.ClusterIP}:{servicePort.Port}";
            cluster.Destinations[clusterKey] = new DestinationConfig()
            {
                Address = string.Empty,
                
            };
        }
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
    private static List<RouteHeader> ConvertHeaderMatch(List<V1beta1HttpHeaderMatch> headerMatches)
    {
        return new List<RouteHeader>();
    }
    private static List<RouteQueryParameter> ConvertQueryParamMatch(List<V1beta1HttpQueryParamMatch> queryParamMatches)
    {
        return new List<RouteQueryParameter>();
    }
}