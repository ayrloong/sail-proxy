using System.Collections.Generic;

namespace Yarp.ReverseProxy.Configuration;

public struct WeightClusterConfig
{
    public List<WeightCluster> Clusters { get; set; }
}