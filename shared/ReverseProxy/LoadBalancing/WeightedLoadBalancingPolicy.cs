using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Yarp.ReverseProxy.Model;

namespace Yarp.ReverseProxy.LoadBalancing;

public class WeightedLoadBalancingPolicy : ILoadBalancingPolicy
{
    public string Name { get; }

    public DestinationState? PickDestination(HttpContext context, ClusterState cluster,
        IReadOnlyList<DestinationState> availableDestinations)
    {
        // return selected destination

        throw new NotImplementedException();
    }
}