using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Sail.Core.Entities;
using Sail.Protocol.Extensions;
using Sail.Protocol.Services;

namespace Sail.Protocol.Apis;

public static class CertificateApi
{
    public static RouteGroupBuilder MapCertificateApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/certificates");

        api.MapGet("/", GetItems);
        api.MapPost("/", Create);
        api.MapPatch("/", Update);
        api.MapDelete("/{id:guid}", Delete);
        return api;
    }
    private static async Task<Results<Ok<IEnumerable<Certificate>>, NotFound>> GetItems(ICertificateService service,
        CancellationToken cancellationToken)
    {
        var items = await service.GetAsync();
        return TypedResults.Ok(items);
    }
    private static async Task<Results<Created, ProblemHttpResult>> Create(ICertificateService service)
    {
        var result = await service.CreateAsync();

        return result.Match<Results<Created, ProblemHttpResult>>(
            created => TypedResults.Created(string.Empty),
            errors => errors.HandleErrors()
        );
    }

    private static async Task<Results<Ok, ProblemHttpResult>> Update(ICertificateService service)
    {
        var result = await service.UpdateAsync();

        return result.Match<Results<Ok, ProblemHttpResult>>(
            created => TypedResults.Ok(),
            errors => errors.HandleErrors()
        );
    }

    private static async Task<Results<Ok, ProblemHttpResult>> Delete(ICertificateService service,Guid id)
    {
        var result = await service.DeleteAsync(id);

        return result.Match<Results<Ok, ProblemHttpResult>>(
            created => TypedResults.Ok(),
            errors => errors.HandleErrors()
        );
    }
}