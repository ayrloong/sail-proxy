using Microsoft.AspNetCore.Http.HttpResults;
using Sail.Extensions;
using Sail.Services;
using Sail.Services;

namespace Sail.Apis;

public static class RouteApi
{
    public static RouteGroupBuilder MapRouteApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/routes").HasApiVersion(1.0);
        
        api.MapGet("/", GetItems);
        api.MapPost("/", Create);
        api.MapPatch("/{id:guid}", Update);
        api.MapDelete("/{id:guid}", Delete);
        return api;
    }

    private static async Task<Results<Ok<IEnumerable<RouteVm>>, NotFound>> GetItems(IRouteService service,
        CancellationToken cancellationToken)
    {
        var items = await service.GetAsync();
        return TypedResults.Ok(items);
    }

    private static async Task<Results<Created, ProblemHttpResult>> Create(IRouteService service,RouteRequest request)
    {
        var result = await service.CreateAsync(request);

        return result.Match<Results<Created, ProblemHttpResult>>(
            created => TypedResults.Created(string.Empty),
            errors => errors.HandleErrors()
        );
    }

    private static async Task<Results<Ok, ProblemHttpResult>> Update(IRouteService service, Guid id,
        RouteRequest request)
    {
        var result = await service.UpdateAsync(id,request);

        return result.Match<Results<Ok, ProblemHttpResult>>(
            created => TypedResults.Ok(),
            errors => errors.HandleErrors()
        );
    }

    private static async Task<Results<Ok, ProblemHttpResult>> Delete(IRouteService service,Guid id)
    {
        var result = await service.DeleteAsync(id);

        return result.Match<Results<Ok, ProblemHttpResult>>(
            created => TypedResults.Ok(),
            errors => errors.HandleErrors()
        );
    }
}

public record RouteRequest(Guid ClusterId, string Name, RouteMatchRequest Match);
public record RouteMatchRequest(string Path,List<string> Methods, List<string> Hosts);