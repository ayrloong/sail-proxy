using Microsoft.EntityFrameworkCore;
using Sail.Core.Entities;
using Sail.Core.Stores;

namespace Sail.EntityFramework.Storage.Stores;

public class ClusterStore(ConfigurationContext context) : IClusterStore
{
    public Task<List<Cluster>> GetAsync()
    {
        return context.Clusters.Include(x=>x.Destinations).ToListAsync();
    }
}