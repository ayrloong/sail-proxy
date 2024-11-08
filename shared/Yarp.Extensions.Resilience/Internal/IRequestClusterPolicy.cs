using Microsoft.AspNetCore.Http;
using Yarp.ReverseProxy.Configuration;

namespace Yarp.Extensions.Resilience.Internal;

public interface IRequestClusterPolicy
{
    WeightedClusterConfig? PickCluster(HttpContext context, IEnumerable<WeightedClusterConfig> clusters);
}