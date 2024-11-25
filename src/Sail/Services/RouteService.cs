using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Sail.Core.Entities;
using Sail.EntityFramework.Storage;
using Sail.Apis;
using Route = Sail.Core.Entities.Route;

namespace Sail.Services;

public class RouteService(ConfigurationContext context) : IRouteService
{
    public async Task<IEnumerable<RouteVm>> GetAsync()
    {
        var result = await context.Routes
            .Include(x => x.Match).ThenInclude(routeMatch => routeMatch.Headers)
            .Include(route => route.Match).ThenInclude(routeMatch => routeMatch.QueryParameters)
            .ToListAsync();

        return result.Select(x => new RouteVm
        {
            Id = x.Id,
            ClusterId = x.ClusterId,
            Name = x.Name,
            Match = new RouteMatchVm
            {
                Id = x.Match.Id,
                Path = x.Match.Path,
                Hosts = x.Match.Hosts,
                Methods = x.Match.Methods,
                Headers = x.Match.Headers.Select(h => new RouteHeaderVm
                {
                    Id = h.Id,
                    Name = h.Name,
                    Mode = h.Mode,
                    Values = h.Values,
                    IsCaseSensitive = h.IsCaseSensitive

                }),
                QueryParameters = x.Match.QueryParameters.Select(q => new RouteQueryParameterVm
                {
                    Id = q.Id,
                    Name = q.Name,
                    Mode = q.Mode,
                    Values = q.Values,
                    IsCaseSensitive = q.IsCaseSensitive
                })
            },
            Order = x.Order,
            AuthorizationPolicy = x.AuthorizationPolicy,
            RateLimiterPolicy = x.RateLimiterPolicy,
            CorsPolicy = x.CorsPolicy,
            TimeoutPolicy = x.TimeoutPolicy,
            Timeout = x.Timeout,
            MaxRequestBodySize = x.MaxRequestBodySize,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        });
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

public record RouteVm
{
    public Guid Id { get; init; }
    public Guid ClusterId { get; init; }
    public string Name { get; init; }
    public RouteMatchVm Match { get; init; }
    public int Order { get; init; }
    public string? AuthorizationPolicy { get; init; }
    public string? RateLimiterPolicy { get; init; }
    public string? CorsPolicy { get; init; }
    public string? TimeoutPolicy { get; init; }
    public TimeSpan? Timeout { get; init; }
    public long? MaxRequestBodySize { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
}

public record RouteMatchVm
{
    public Guid Id { get; init; }
    public List<string> Methods { get; init; }
    public List<string> Hosts { get; init; }
    public IEnumerable<RouteHeaderVm> Headers  { get; init; }
    public string Path { get; init; }
    public IEnumerable<RouteQueryParameterVm> QueryParameters { get; init; }
}

public record RouteHeaderVm
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public List<string> Values { get; init; }
    public HeaderMatchMode Mode { get; init; }
    public bool IsCaseSensitive { get; init; }
}

public class RouteQueryParameterVm
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public List<string> Values { get; init; }
    public QueryParameterMatchMode Mode { get; init; }
    public bool IsCaseSensitive { get; init; }
}