namespace Sail.Core.Entities;

public class WeightedCluster
{
    public Guid Id { get; set; }
    public string ClusterId { get; set; }
    public int Weight { get; set; }
}