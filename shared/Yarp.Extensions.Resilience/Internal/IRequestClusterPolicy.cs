using Microsoft.AspNetCore.Http;

namespace Yarp.Extensions.Resilience.Internal;

public interface IRequestClusterPolicy
{
  //  WeightedClusterConfig? PickCluster(HttpContext context, IEnumerable<WeightedClusterConfig> clusters);
}