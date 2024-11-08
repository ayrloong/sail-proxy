using Sail.Core.Entities;

namespace Sail.Core.Stores;

public interface IClusterStore
{
    Task<List<Cluster>> GetAsync();
}