using System.Collections.Immutable;
using Sail.Api.V1;
using Sail.Core.Certificates;
using Yarp.ReverseProxy.Configuration;
using RouteMatch = Yarp.ReverseProxy.Configuration.RouteMatch;

namespace Sail.Compass.Converters;

internal static class Parser
{
    internal static IReadOnlyList<CertificateConfig> ConvertCertificates(IEnumerable<Certificate> certificates)
    {
        throw new NotImplementedException();
    }

    internal static void ConvertFromDataSource(DataSourceContext dataSourceContext, YarpConfigContext configContext)
    {
        foreach (var route in dataSourceContext.Routes)
        {
            HandleRoute(configContext, route);
        }

        foreach (var cluster in dataSourceContext.Clusters)
        {
            HandleCluster(configContext, cluster);
        }
    }

    private static void HandleCluster(YarpConfigContext configContext, Cluster cluster)
    {
        var clusters = configContext.Clusters;

        clusters.Add(new ClusterConfig
        {
            ClusterId = cluster.ClusterId,
            LoadBalancingPolicy = cluster.LoadBalancingPolicy,
            Destinations = cluster.Destinations?.ToDictionary(x => x.DestinationId, x => new DestinationConfig
            {
                Host = x.Host,
                Health = x.Health,
                Address = x.Address
            })
        });
    }

    private static void HandleRoute(YarpConfigContext configContext, Route route)
    {
        var routes = configContext.Routes;

        routes.Add(new RouteConfig
        {
            RouteId = route.RouteId,
            ClusterId = route.ClusterId,
            Match = new RouteMatch
            {
                Hosts = route.Match.Hosts,
                Path = route.Match.Path,
                Methods = route.Match.Methods,
                // Headers = route.Match.Headers.Select(x => x.ToRouteHeader()).ToList(),
                // QueryParameters = route.Match.QueryParameters.Select(x => x.ToRouteQueryParameter()).ToList()
            },
            AuthorizationPolicy = route.AuthorizationPolicy,
            RateLimiterPolicy = route.RateLimiterPolicy,
            TimeoutPolicy = route.TimeoutPolicy,
            CorsPolicy = route.CorsPolicy,
            //Timeout = TimeSpan.FromSeconds(route.Timeout),
            MaxRequestBodySize = route.MaxRequestBodySize,
            Order = route.Order
        });
    }
}