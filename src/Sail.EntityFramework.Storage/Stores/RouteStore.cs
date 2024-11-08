using Microsoft.EntityFrameworkCore;
using Sail.Core.Entities;
using Sail.Core.Stores;

namespace Sail.EntityFramework.Storage.Stores;

public class RouteStore(ConfigurationContext context) : IRouteStore
{
    public Task<List<Route>> GetAsync()
    {
        return context.Routes.Include(x=>x.Match)
            .ThenInclude(x=>x.QueryParameters).ToListAsync();
    }
}