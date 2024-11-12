namespace Sail.Core.Entities;

public class WeightedCluster
{
    public Guid Id { get; set; }
    public Guid ClusterId { get; set; }
    public int Weight { get; set; }
}