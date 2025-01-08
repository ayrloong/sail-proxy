using Microsoft.AspNetCore.Http.HttpResults;
using Sail.Extensions;
using Sail.Services;

namespace Sail.Apis;

public static class CertificateApi
{
    public static RouteGroupBuilder MapCertificateApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/certificates").HasApiVersion(1.0);

        api.MapGet("/", GetItems);
        api.MapPost("/", Create);
        api.MapPatch("/", Update);
        api.MapDelete("/{id:guid}", Delete);

        api.MapGet("/{certificateId:guid}/snis", GetSNIs);
        api.MapPost("/{certificateId:guid}/snis", CreateSNI);
        api.MapPost("/{certificateId:guid}/snis/{id:guid}", UpdateSNI);
        api.MapDelete("/{certificateId:guid}/snis/{id:guid}", DeleteSNI);
        return api;
    }

    private static async Task<Results<Ok<IEnumerable<SNIVm>>, NotFound>> GetSNIs(ICertificateService service,
        Guid certificateId,
        CancellationToken cancellationToken)
    {
        var items = await service.GetSNIsAsync(certificateId, cancellationToken);
        return TypedResults.Ok(items);
    }

    private static async Task<Results<Created, ProblemHttpResult>> CreateSNI(ICertificateService service,
        Guid certificateId, SNIRequest request, CancellationToken cancellationToken)
    {
        var result = await service.CreateSNIAsync(certificateId, request, cancellationToken);

        return result.Match<Results<Created, ProblemHttpResult>>(
            created => TypedResults.Created(string.Empty),
            errors => errors.HandleErrors()
        );
    }

    private static async Task<Results<Ok, ProblemHttpResult>> UpdateSNI(ICertificateService service, Guid certificateId,
        Guid id, SNIRequest request, CancellationToken cancellationToken)
    {
        var result = await service.UpdateSNIAsync(certificateId, id, request, cancellationToken);

        return result.Match<Results<Ok, ProblemHttpResult>>(
            created => TypedResults.Ok(),
            errors => errors.HandleErrors()
        );
    }

    private static async Task<Results<Ok, ProblemHttpResult>> DeleteSNI(ICertificateService service, Guid certificateId,
        Guid id, CancellationToken cancellationToken)
    {
        var result = await service.DeleteSNIAsync(certificateId, id, cancellationToken);

        return result.Match<Results<Ok, ProblemHttpResult>>(
            created => TypedResults.Ok(),
            errors => errors.HandleErrors()
        );
    }

    private static async Task<Results<Ok<IEnumerable<CertificateVm>>, NotFound>> GetItems(ICertificateService service,
        CancellationToken cancellationToken)
    {
        var items = await service.GetAsync(cancellationToken);
        return TypedResults.Ok(items);
    }

    private static async Task<Results<Created, ProblemHttpResult>> Create(ICertificateService service,
        CertificateRequest request, CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request, cancellationToken);

        return result.Match<Results<Created, ProblemHttpResult>>(
            created => TypedResults.Created(string.Empty),
            errors => errors.HandleErrors()
        );
    }

    private static async Task<Results<Ok, ProblemHttpResult>> Update(ICertificateService service, Guid id,
        CertificateRequest request, CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(id, request, cancellationToken);

        return result.Match<Results<Ok, ProblemHttpResult>>(
            created => TypedResults.Ok(),
            errors => errors.HandleErrors()
        );
    }

    private static async Task<Results<Ok, ProblemHttpResult>> Delete(ICertificateService service, Guid id,
        CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(id, cancellationToken);

        return result.Match<Results<Ok, ProblemHttpResult>>(
            created => TypedResults.Ok(),
            errors => errors.HandleErrors()
        );
    }
}

public record CertificateRequest(string Cert,string Key);
public record SNIRequest(string HostName, string Name);