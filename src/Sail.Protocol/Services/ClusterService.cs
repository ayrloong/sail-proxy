using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Sail.Core.Entities;
using Sail.EntityFramework.Storage;

namespace Sail.Protocol.Services;

public class ClusterService(ConfigurationContext context) : IClusterService
{
    public async Task<IEnumerable<Cluster>> GetAsync()
    {
        return await context.Clusters.ToListAsync();
    }

    public async Task<ErrorOr<Created>> CreateAsync()
    {
        var cluster = new Cluster();
        await context.Clusters.AddAsync(cluster);
        await context.SaveChangesAsync();
        return Result.Created;
    }

    public async Task<ErrorOr<Updated>> UpdateAsync()
    {
        var cluster = await context.Clusters.SingleOrDefaultAsync(x => x.Id == new Guid());
        if (cluster is null)
        {
            return Error.NotFound(description: "Cluster not found");
        }

        context.Clusters.Update(cluster);
        await context.SaveChangesAsync();
        return Result.Updated;
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(Guid id)
    {
        var cluster = await context.Clusters.SingleOrDefaultAsync(x => x.Id == id);
        if (cluster is null)
        {
            return Error.NotFound(description: "Cluster not found");
        }

        context.Clusters.Remove(cluster);
        await context.SaveChangesAsync();
        return Result.Deleted;
    }
}