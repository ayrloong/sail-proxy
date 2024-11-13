using Microsoft.EntityFrameworkCore;
using Sail.Core.Entities;
using Sail.Core.Stores;

namespace Sail.EntityFramework.Storage.Stores;

public class RouteStore(ConfigurationContext context) : IRouteStore
{
    public Task<List<Route>> GetAsync()
    {
        return context.Routes.Include(x => x.Match).ThenInclude(routeMatch => routeMatch.Headers)
            .Include(route => route.Match).ThenInclude(routeMatch => routeMatch.QueryParameters)
            .ToListAsync();
    }
}