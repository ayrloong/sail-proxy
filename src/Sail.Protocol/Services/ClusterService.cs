using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Sail.Core.Entities;
using Sail.EntityFramework.Storage;
using Sail.Protocol.Apis;

namespace Sail.Protocol.Services;

public class ClusterService(ConfigurationContext context) : IClusterService
{
    public async Task<IEnumerable<Cluster>> GetAsync()
    {
        return await context.Clusters.ToListAsync();
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