using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;

namespace Sail.Protocol.Apis;

public static class CertificateApi
{
    public static IEndpointRouteBuilder MapCertificateApiV1(this IEndpointRouteBuilder app)
    {
        app.MapPost("/", Create);
        return app;
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