using Sail.Core.Entities;

namespace Sail.Core.Stores;

public interface IRouteStore
{
    Task<List<Route>> GetAsync();
}