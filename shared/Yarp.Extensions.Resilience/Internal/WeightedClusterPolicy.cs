using Microsoft.AspNetCore.Http;
using Yarp.ReverseProxy.Configuration;
using Yarp.Extensions.Resilience.Utilities;

namespace Yarp.Extensions.Resilience.Internal;

internal class WeightedClusterPolicy(Randomizer randomizer) : IRequestClusterPolicy
{
    public WeightedClusterConfig? PickCluster(HttpContext context, IEnumerable<WeightedClusterConfig> clusters)
    {
        return clusters.SelectByWeight(g => g.Weight ?? 0, randomizer);
    }
}