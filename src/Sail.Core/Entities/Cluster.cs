namespace Sail.Core.Entities;

public class Cluster
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? LoadBalancingPolicy { get; set; }
    public List<Destination>? Destinations { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}