namespace Sail.Configuration;

public class CanaryConfig
{
    public bool? Enabled { get; init; }
    public int Weight { get; set; }
    public string? CustomHeader { get; set; }
    public string? CustomHeaderValue { get; set; }
}