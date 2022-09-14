namespace Yarp.ReverseProxy.Configuration;

public sealed record CanaryConfig
{
    public bool? Enabled { get; init; }
    public int? Weight { get; set; }
    public string? CustomHeader { get; set; }
    public string? CustomHeaderValue { get; set; }
}