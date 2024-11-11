using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Sail.Core.Entities;
using Sail.EntityFramework.Storage;

namespace Sail.Protocol.Services;

public class RouteService(ConfigurationContext context) : IRouteService
{
    public async Task<IEnumerable<Route>> GetAsync()
    {
        return await context.Routes.ToListAsync();
    }

    public async Task<ErrorOr<Created>> CreateAsync()
    {
        var route = new Route();
        await context.Routes.AddAsync(route);
        await context.SaveChangesAsync();
        return Result.Created;
    }

    public async Task<ErrorOr<Updated>> UpdateAsync()
    {
        var route = await context.Routes.SingleOrDefaultAsync(x => x.Id == new Guid());
        if (route is null)
        {
            return Error.NotFound(description: "Route not found");
        }

        context.Routes.Update(route);
        await context.SaveChangesAsync();
        return Result.Updated;
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(Guid id)
    {
        var route = await context.Routes.SingleOrDefaultAsync(x => x.Id == id);

        if (route is null)
        {
            return Error.NotFound(description: "Route not found");
        }

        context.Routes.Remove(route);
        await context.SaveChangesAsync();
        return Result.Deleted;
    }
}