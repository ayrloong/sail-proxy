using ErrorOr;
using Sail.Apis;
using Sail.Core.Entities;
using MongoDB.Driver;
using Sail.Storage.MongoDB;

namespace Sail.Services;

public class ClusterService(SailContext context) : IClusterService
{
    public async Task<IEnumerable<ClusterVm>> GetAsync(CancellationToken cancellationToken = default)
    {
        var filter = Builders<Cluster>.Filter.Empty;
        var routes = await context.Clusters.FindAsync(filter, cancellationToken: cancellationToken);
        var items = await routes.ToListAsync(cancellationToken: cancellationToken);

        return items.Select(MapToClusterVm);
    }

    public async Task<ErrorOr<Created>> CreateAsync(ClusterRequest request,CancellationToken cancellationToken = default)
    {
        var cluster = new Cluster
        {
            Name = request.Name,
            LoadBalancingPolicy = request.LoadBalancingPolicy,
            Destinations = request.Destinations.Select(item => new Destination
            {
                Host = item.Host,
                Address = item.Address,
                Health = item.Health
            }).ToList()
        };
        await context.Clusters.InsertOneAsync(cluster, cancellationToken: cancellationToken);
        return Result.Created;
    }

    public async Task<ErrorOr<Updated>> UpdateAsync(Guid id, ClusterRequest request,CancellationToken cancellationToken = default)
    {
        var filter = Builders<Cluster>.Filter.And(Builders<Cluster>.Filter.Where(x => x.Id == id));
        
        var update = Builders<Cluster>.Update
            .Set(x => x.Name,request.Name)
            .Set(x => x.LoadBalancingPolicy, request.LoadBalancingPolicy)
            .Set(x => x.UpdatedAt, DateTimeOffset.UtcNow);

        await context.Clusters.FindOneAndUpdateAsync(filter, update, cancellationToken: cancellationToken);
        return Result.Updated;
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(Guid id,CancellationToken cancellationToken = default)
    {
        var filter = Builders<Cluster>.Filter.And(Builders<Cluster>.Filter.Where(x => x.Id == id));
        await context.Clusters.DeleteOneAsync(filter, cancellationToken);
        return Result.Deleted;
    }

    private ClusterVm MapToClusterVm(Cluster cluster)
    {
        return new ClusterVm
        {
            Id = cluster.Id,
            Name = cluster.Name,
            LoadBalancingPolicy = cluster.LoadBalancingPolicy,
            Destinations = cluster.Destinations?.Select(d => new DestinationVm
            {
                Host = d.Host,
                Address = d.Address,
                Health = d.Health
            }),
            CreatedAt = cluster.CreatedAt,
            UpdatedAt = cluster.UpdatedAt
        };
    }
}

public record ClusterVm
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string? LoadBalancingPolicy { get; init; }
    public IEnumerable<DestinationVm>? Destinations { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
}

public record DestinationVm
{
    public string Address { get; init; }
    public string? Health { get; init; }
    public string? Host { get; init; }
}