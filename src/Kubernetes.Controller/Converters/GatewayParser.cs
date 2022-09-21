using Sail.Kubernetes.Controller.Models;
using Yarp.ReverseProxy.Configuration;

namespace Sail.Kubernetes.Controller.Converters;

public class GatewayParser
{
    internal static void ConvertFromKubernetesGateway(GatewayContext gatewayContext,ConfigContext configContext)
    {
        var httpRoutes = gatewayContext.HttpRoutes;
        var hostnames = httpRoutes.SelectMany(x => x.Spec.Hostnames).ToList();
        var rules = httpRoutes.SelectMany(x => x.Spec.Rules);
       
        foreach (var rule in  rules)
        {
            HandleGatewayRule(gatewayContext, rule, hostnames, configContext);
        }
    }

    private static void HandleGatewayRule(GatewayContext gatewayContext,
        V1beta1HttpRouteRule rule,
        List<string> hostnames,
        ConfigContext configContext)
    {
        var clusters = configContext.ClusterTransfers;
        var routes = configContext.Routes;

        var pathMatches = rule.Matches.Where(x => x.Path != null);
        var methodMatches = rule.Matches.Where(x => x.Method != null);
        var headersMatches = rule.Matches.Where(x => x.Headers != null);

    }
    
}