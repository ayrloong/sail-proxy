using System.Collections.Immutable;
using Sail.Core.Certificates;
using Sail.Core.Entities;
using Yarp.ReverseProxy.Configuration;
using RouteMatch = Yarp.ReverseProxy.Configuration.RouteMatch;

namespace Sail.Core.Converters;

internal static class Parser
{
    internal static IReadOnlyList<CertificateConfig> ConvertCertificates(IEnumerable<Certificate> certificates)
    {
        return certificates.Select(x => new CertificateConfig
        {
            Cert = x.Cert
        }).ToImmutableList();
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
            ClusterId = cluster.Id.ToString(),
            LoadBalancingPolicy = cluster.LoadBalancingPolicy,
            Destinations = cluster.Destinations.ToDictionary(x => x.Id.ToString(), x => new DestinationConfig
            {
                Address = x.Address
            })
        });
    }

    private static void HandleRoute(YarpConfigContext configContext, Route route)
    {
        var routes = configContext.Routes;

        routes.Add(new RouteConfig
        {
            RouteId = route.Id.ToString(),
            ClusterId = route.ClusterId.ToString(),
            Match = new RouteMatch
            {
                Hosts = route.Match.Hosts,
                Path = route.Match.Path,
                Methods = route.Match.Methods,
                Headers = route.Match.Headers?.Select(x => x.ToRouteHeader()).ToList(),
                QueryParameters = route.Match.QueryParameters?.Select(x => x.ToRouteQueryParameter()).ToList()
            },
            WeightedClusters = route.WeightedClusters?.Select(w => new WeightedClusterConfig
            {
                ClusterId = w.ClusterId,
                Weight = w.Weight
            }).ToList(),
            AuthorizationPolicy = route.AuthorizationPolicy,
            RateLimiterPolicy = route.RateLimiterPolicy,
            TimeoutPolicy = route.TimeoutPolicy,
            CorsPolicy = route.CorsPolicy,
            Timeout = route.Timeout,
            MaxRequestBodySize = route.MaxRequestBodySize,
            Order = route.Order
        });
    }
}