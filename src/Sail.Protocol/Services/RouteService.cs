using Microsoft.EntityFrameworkCore;
using Sail.EntityFramework.Storage;

namespace Sail.Protocol.Services;

public class RouteService(ConfigurationContext context) : IRouteService
{
    public async Task<IEnumerable<Core.Entities.Route>> GetAsync()
    {
        return await context.Routes.ToListAsync();
    }
}