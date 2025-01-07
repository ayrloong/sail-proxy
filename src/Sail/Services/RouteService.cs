using ErrorOr;
using MongoDB.Driver;
using Sail.Core.Entities;
using Sail.Storage.MongoDB;
using Sail.Apis;
using Route = Sail.Core.Entities.Route;


namespace Sail.Services;

public class RouteService(SailContext context) : IRouteService
{
    public async Task<IEnumerable<RouteVm>> GetAsync(CancellationToken cancellationToken = default)
    {
        var filter = Builders<Route>.Filter.Empty;
        var routes = await context.Routes.FindAsync(filter, cancellationToken: cancellationToken);
        var items = await routes.ToListAsync(cancellationToken: cancellationToken);
        return items.Select(MapToRouteVm);
    }

    public async Task<ErrorOr<Created>> CreateAsync(RouteRequest request,CancellationToken cancellationToken = default)
    {
        var route = new Route
        {
            Name = request.Name,
          
        };
        
        await context.Routes.InsertOneAsync(route, cancellationToken: cancellationToken);
        return Result.Created;
    }

    public async Task<ErrorOr<Updated>> UpdateAsync(Guid id, RouteRequest request,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<Route>.Filter.And(Builders<Route>.Filter.Where(x => x.Id == id));
        
        var update = Builders<Route>.Update
            .Set(x => x.Name,request.Name)
            .Set(x => x.ClusterId, request.ClusterId)
            .Set(x => x.UpdatedAt, DateTimeOffset.UtcNow);

        await context.Routes.FindOneAndUpdateAsync(filter, update, cancellationToken: cancellationToken);
        return Result.Updated;
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Route>.Filter.And(Builders<Route>.Filter.Where(x => x.Id == id));
        await context.Routes.DeleteOneAsync(filter, cancellationToken);
        return Result.Deleted;
    }

    private RouteVm MapToRouteVm(Route route)
    {
        return new RouteVm
        {
            Id = route.Id,
            Name = route.Name,
            Match = new RouteMatchVm
            {
                Id = route.Match.Id,
                Path = route.Match.Path,
                Hosts = route.Match.Hosts,
                Methods = route.Match.Methods,
                Headers = route.Match.Headers.Select(h => new RouteHeaderVm
                {
                    Id = h.Id,
                    Name = h.Name,
                    Mode = h.Mode,
                    Values = h.Values,
                    IsCaseSensitive = h.IsCaseSensitive

                }),
                QueryParameters = route.Match.QueryParameters.Select(q => new RouteQueryParameterVm
                {
                    Id = q.Id,
                    Name = q.Name,
                    Mode = q.Mode,
                    Values = q.Values,
                    IsCaseSensitive = q.IsCaseSensitive
                })
            },
            Order = route.Order,
            AuthorizationPolicy = route.AuthorizationPolicy,
            RateLimiterPolicy = route.RateLimiterPolicy,
            CorsPolicy = route.CorsPolicy,
            TimeoutPolicy = route.TimeoutPolicy,
            Timeout = route.Timeout,
            MaxRequestBodySize = route.MaxRequestBodySize,
            CreatedAt = route.CreatedAt,
            UpdatedAt = route.UpdatedAt
        };
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