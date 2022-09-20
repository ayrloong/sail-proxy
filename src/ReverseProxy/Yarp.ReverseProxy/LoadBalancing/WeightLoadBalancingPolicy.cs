using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Yarp.ReverseProxy.Model;

namespace Yarp.ReverseProxy.LoadBalancing;

internal sealed class WeightLoadBalancingPolicy : ILoadBalancingPolicy
{
    public string Name => LoadBalancingPolicies.Weight;
    
    public DestinationState? PickDestination(HttpContext context, ClusterState cluster,
        IReadOnlyList<DestinationState> availableDestinations)
    {
        var dictionary = availableDestinations.ToDictionary(x => x, x => (float)x.Model.Config.Weight / 100);
        var destination = dictionary.Weight(x => x.Value).Key;
        return destination;
    }
}