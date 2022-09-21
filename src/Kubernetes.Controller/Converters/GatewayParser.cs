using k8s.Models;
using Sail.Kubernetes.Controller.Caching;
using Sail.Kubernetes.Controller.Models;
using Yarp.ReverseProxy.Configuration;

namespace Sail.Kubernetes.Controller.Converters;

public class GatewayParser
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
        List<string> hostnames,
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
        List<string> hostnames,
        ConfigContext configContext)
    {

        var pathMatches = rule.Matches.Where(x => x.Path != null).Select(x => x.Path).ToList();
        var methodMatches= rule.Matches.Where(x => x.Method != null).Select(x => x.Method).ToList();
        var queryParamMatches = rule.Matches.Where(x => x.QueryParams != null).Select(x => x.QueryParams).ToList();
        var headerMatches = rule.Matches.Where(x => x.Headers != null).SelectMany(x => x.Headers).ToList();
 
        if (!pathMatches.Any())
        {
            var pathMatch = new V1beta1HttpPathMatch
            {
                Type = string.Empty,
                Value = string.Empty
            };
            pathMatches.Add(pathMatch);
        }

        
        foreach (var pathMatch in pathMatches)
        {
            HandleGatewayRulePath(gatewayContext, pathMatch, hostnames, methodMatches, queryParamMatches, headerMatches,
                rule.BackendRefs,
                configContext);
        }
    }

    private static void HandleGatewayRulePath(GatewayContext gatewayContext,
        V1beta1HttpPathMatch path,
        List<string> hostnames,
        List<string> methodMatches,
        List<V1beta1HttpQueryParamMatch> queryParamMatches,
        List<V1beta1HttpHeaderMatch> headerMatches,
        List<V1beta1HttpBackendRef> backendRefs,
        ConfigContext configContext)
    {
        var clusters = configContext.ClusterTransfers;
        var routes = configContext.Routes;

        var key = string.Empty;
        if (!clusters.ContainsKey(key))
        {
            clusters.Add(key, new ClusterTransfer());
        }

        var cluster = clusters[key];
        cluster.ClusterId = key;
        
        var headers = ConvertHeaderMatch(headerMatches);
        var queryParameters = ConvertQueryParamMatch(queryParamMatches);

        var routeConfig = new RouteConfig()
        {
            Match = new RouteMatch()
            {
                Hosts = hostnames,
                Path = path.Value,
                Methods = methodMatches,
                Headers = headers,
                QueryParameters = queryParameters
            }
        };

        foreach (var backendRef in backendRefs)
        {
            var protocol = "https";
            var service = gatewayContext.Services.SingleOrDefault(s => s.Metadata.Name == backendRef.Name);
            var servicePort = service.Spec.Ports.SingleOrDefault(p => MatchesPort(p, backendRef));
            var uri = $"{protocol}://{service.Spec.ClusterIP}:{servicePort.Port}";
            cluster.Destinations[uri] = new DestinationConfig()
            {
                Address = uri
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