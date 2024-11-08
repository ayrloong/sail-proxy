namespace Yarp.ReverseProxy.Configuration;

public sealed record WeightedClusterConfig
{
    private const int MinWeight = 1;
    private const int MaxWeight = 64000;
    private const int DefaultWeight = MaxWeight / 2;

    public string? ClusterId { get; init; }

    public int? Weight { get; set; } = DefaultWeight;
}