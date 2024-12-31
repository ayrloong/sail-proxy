using Microsoft.AspNetCore.Http;

namespace Yarp.Extensions.Resilience;

public class ResilienceDelegatingHandler(
    IResiliencePipelineProvider<HttpResponseMessage> pipelineProvider,
    IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {

        var httpContext = httpContextAccessor.HttpContext;
        var feature = httpContext?.GetReverseProxyFeature();
        var pipeline = pipelineProvider.GetResiliencePipeline("");

        if (pipeline is null)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        return await pipeline.ExecuteAsync(async token => await base.SendAsync(request, token), cancellationToken);
    }
}
 