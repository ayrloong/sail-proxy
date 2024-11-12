namespace Sail.Core.Entities;

public class Destination
{
    public Guid Id { get; set; }
    public Guid ClusterId { get; set; }
    public Cluster Cluster { get; set; }
    public string Address { get; set; }
    public string? Health { get; set; }
    public string? Host { get; set; }
}