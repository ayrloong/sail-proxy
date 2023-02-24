namespace Yarp.ReverseProxy.Configuration;

public struct WeightCluster
{
    public string ClusterId  { get; set; }
    public int? Weight { get; set; }
}