using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Sail.Core.Entities;
using Sail.Protocol.Extensions;
using Sail.Protocol.Services;

namespace Sail.Protocol.Apis;

public static class ClusterApi
{
    public static RouteGroupBuilder MapClusterApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/clusters");

        api.MapGet("/", GetItems);
        api.MapPost("/", Create);
        api.MapPatch("/", Update);
        api.MapDelete("/{id:guid}", Delete);
        return api;
    }

    private static async Task<Results<Ok<IEnumerable<Cluster>>, NotFound>> GetItems(IClusterService service,
        CancellationToken cancellationToken)
    {
        var items = await service.GetAsync();
        return TypedResults.Ok(items);
    }

    private static async Task<Results<Created, ProblemHttpResult>> Create(IClusterService service)
    {
        var result = await service.CreateAsync();

        return result.Match<Results<Created, ProblemHttpResult>>(
            created => TypedResults.Created(string.Empty),
            errors => errors.HandleErrors()
        );
    }

    private static async Task<Results<Ok, ProblemHttpResult>> Update(IClusterService service)
    {
        var result = await service.UpdateAsync();

        return result.Match<Results<Ok, ProblemHttpResult>>(
            created => TypedResults.Ok(),
            errors => errors.HandleErrors()
        );
    }

    private static async Task<Results<Ok, ProblemHttpResult>> Delete(IRouteService service, Guid id)
    {
        var result = await service.DeleteAsync(id);

        return result.Match<Results<Ok, ProblemHttpResult>>(
            created => TypedResults.Ok(),
            errors => errors.HandleErrors()
        );
    }
}