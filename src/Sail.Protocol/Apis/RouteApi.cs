using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Sail.Protocol.Services;

namespace Sail.Protocol.Apis;

public static class RouteApi
{
    public static IEndpointRouteBuilder MapRouteApiV1(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", GetItems);
        app.MapPost("/", Create);
        return app;
    }

    private static async Task<Results<Ok<IEnumerable<Core.Entities.Route>>, NotFound>> GetItems(IRouteService service,
        CancellationToken cancellationToken)
    {
        var items = await service.GetAsync();
        return TypedResults.Ok(items);
    }

    private static async Task<Created> Create()
    {
        return TypedResults.Created("");
    }
    
    private static async Task<Created> Update()
    {

        return TypedResults.Created("");
    }

    private static async Task<Results<NoContent, NotFound>> Delete()
    {
        return TypedResults.NoContent();
    }
}