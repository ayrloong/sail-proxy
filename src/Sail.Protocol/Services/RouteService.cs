using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Sail.Core.Entities;
using Sail.EntityFramework.Storage;
using Sail.Protocol.Apis;

namespace Sail.Protocol.Services;

public class RouteService(ConfigurationContext context) : IRouteService
{
    public async Task<IEnumerable<Route>> GetAsync()
    {
        return await context.Routes.ToListAsync();
    }

    public async Task<ErrorOr<Created>> CreateAsync(RouteRequest request)
    {
        var route = new Route
        {
            Name = request.Name,
            ClusterId = request.ClusterId,
            Match = new RouteMatch
            {
                Path = request.Match.Path,
                Hosts = request.Match.Hosts,
                Methods = request.Match.Methods
            }
        };
        await context.Routes.AddAsync(route);
        await context.SaveChangesAsync();
        return Result.Created;
    }

    public async Task<ErrorOr<Updated>> UpdateAsync(Guid id, RouteRequest request)
    {
        var route = await context.Routes.FindAsync(id);
        if (route is null)
        {
            return Error.NotFound(description: "Route not found");
        }

        route.Name = request.Name;
        route.ClusterId = request.ClusterId;
        route.UpdatedAt = DateTimeOffset.Now;
        context.Routes.Update(route);
        await context.SaveChangesAsync();
        return Result.Updated;
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(Guid id)
    {
        var route = await context.Routes.FindAsync(id);

        if (route is null)
        {
            return Error.NotFound(description: "Route not found");
        }

        context.Routes.Remove(route);
        await context.SaveChangesAsync();
        return Result.Deleted;
    }
}