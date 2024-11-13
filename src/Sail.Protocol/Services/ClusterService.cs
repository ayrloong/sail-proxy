using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Sail.Core.Entities;
using Sail.EntityFramework.Storage;
using Sail.Protocol.Apis;

namespace Sail.Protocol.Services;

public class ClusterService(ConfigurationContext context) : IClusterService
{
    public async Task<IEnumerable<ClusterVm>> GetAsync()
    {
        var items = await context.Clusters.Include(x => x.Destinations).ToListAsync();

        return items.Select(x => new ClusterVm
        {
            Id = x.Id,
            Name = x.Name,
            LoadBalancingPolicy = x.LoadBalancingPolicy,
            Destinations = x.Destinations?.Select(d => new DestinationVm
            {
                Id = d.Id,
                Host = d.Host,
                Address = d.Address,
                Health = d.Health
            }),
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        });
    }

    public async Task<ErrorOr<Created>> CreateAsync(ClusterRequest request)
    {
        var cluster = new Cluster
        {
            Name = request.Name,
            LoadBalancingPolicy = request.LoadBalancingPolicy,
            Destinations = request.Destinations.Select(item => new Destination
            {
                Host = item.Host,
                Address = item.Address,
                Health = item.Health
            }).ToList()
        };
        await context.Clusters.AddAsync(cluster);
        await context.SaveChangesAsync();
        return Result.Created;
    }

    public async Task<ErrorOr<Updated>> UpdateAsync(Guid id, ClusterRequest request)
    {
        var cluster = await context.Clusters.FindAsync(id);
        if (cluster is null)
        {
            return Error.NotFound(description: "Cluster not found");
        }

        cluster.Name = request.Name;
        cluster.LoadBalancingPolicy = request.LoadBalancingPolicy;
        cluster.Destinations = request.Destinations.Select(item => new Destination
        {
            Host = item.Host,
            Address = item.Address,
            Health = item.Health
        }).ToList();
        cluster.UpdatedAt = DateTimeOffset.Now;
        
        context.Clusters.Update(cluster);
        await context.SaveChangesAsync();
        return Result.Updated;
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(Guid id)
    {
        var cluster = await context.Clusters.FindAsync(id);
        if (cluster is null)
        {
            return Error.NotFound(description: "Cluster not found");
        }

        context.Clusters.Remove(cluster);
        await context.SaveChangesAsync();
        return Result.Deleted;
    }
}

public record ClusterVm
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string? LoadBalancingPolicy { get; init; }
    public IEnumerable<DestinationVm>? Destinations { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
}

public record DestinationVm
{
    public Guid Id { get; init; }
    public string Address { get; init; }
    public string? Health { get; init; }
    public string? Host { get; init; }
}