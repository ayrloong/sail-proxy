using System.Net;
using Polly.CircuitBreaker;

namespace YarpSample;

public class CustomDelegatingHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        try
        {
            var httpContext = httpContextAccessor.HttpContext;
            var proxyFeature = httpContext.GetReverseProxyFeature();
            return await base.SendAsync(request, cancellationToken);
            
        }
        catch (BrokenCircuitException ex)
        {
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadGateway
            };
        }
        catch (HttpRequestException ex)
        {
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadGateway
            };
        }
    }
}