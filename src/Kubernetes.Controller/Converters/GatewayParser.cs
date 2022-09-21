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
        var pathMatches = rule.Matches.Where(x => x.Path != null);
        var methodMatches= rule.Matches.Where(x => x.Method != null);
        var queryParamMatches = rule.Matches.Select(x => x.QueryParams != null);
        var headerMatches = rule.Matches.Select(x => x.Headers != null);

        foreach (var path in pathMatches)
        {
            
        }
    }

    private static void HandleGatewayRulePath()
    {
        
    }
}