using Sail.Kubernetes.Controller.Caching;
using Sail.Kubernetes.Controller.Models;
using Sail.Kubernetes.Protocol.Configuration;
using Yarp.ReverseProxy.Configuration;

namespace Sail.Kubernetes.Controller.Converters;

internal class SailConfigContext
{
    public Dictionary<string, ClusterTransfer> ClusterTransfers { get; set; } = new();
    public List<RouteConfig> Routes { get; set; } = new();

    public List<ClusterConfig> BuildClusterConfig()
    {
        return ClusterTransfers.Values.Select(c => new ClusterConfig
        {
            Destinations = c.Destinations,
            ClusterId = c.ClusterId,
            HealthCheck = c.HealthCheck,
            LoadBalancingPolicy = c.LoadBalancingPolicy,
            SessionAffinity = c.SessionAffinity,
            HttpClient = c.HttpClientConfig
        }).ToList();
    }

    public List<MiddlewareConfig> BuildMiddlewareConfig(List<MiddlewareData> middlewares)
    {

        return middlewares
            .Where(x => x.Spec.Cors is not null || x.Spec.JwtBearer is not null || x.Spec.RateLimiter is not null)
            .Select(m => new MiddlewareConfig
            {
                JwtBearer = BuildJwtBearer(m.Spec.JwtBearer, m.Metadata.Name),
                Cors = BuildCors(m.Spec.Cors, m.Metadata.Name),
                RateLimiter = BuildRateLimiter(m.Spec.RateLimiter, m.Metadata.Name)
            }).ToList();
    }

    private JwtBearerConfig BuildJwtBearer(JwtBearer jwtBearer, string name)
    {
        if (jwtBearer is null)
        {
            return null;
        }

        return new JwtBearerConfig
        {
            Name = name,
            Audience = jwtBearer.Audience,
            Secret = jwtBearer.Secret,
            Issuer = jwtBearer.Issuer,
        };
    }

    private CorsConfig BuildCors(Cors cors, string name)
    {
        if (cors is null)
        {
            return null;
        }

        return new CorsConfig
        {
            Name = name,
            AllowOrigins = cors.AllowOrigins,
            AllowMethods = cors.AllowMethods,
            AllowHeaders = cors.AllowHeaders
        };
    }

    private RateLimiterConfig BuildRateLimiter(RateLimiter rateLimiter, string name)
    {
        if (rateLimiter is null)
        {
            return null;
        }

        return new RateLimiterConfig
        {
            Name = name,
            PermitLimit = rateLimiter.PermitLimit,
            Window = rateLimiter.Window,
            QueueLimit = rateLimiter.QueueLimit
        };
    }
}