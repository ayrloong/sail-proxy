using Microsoft.AspNetCore.Http;

namespace Sail.Model;

public class PipelineInitializerMiddleware
{
    private readonly RequestDelegate _next;

    public PipelineInitializerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context)
    {
        var endpoint = context.GetEndpoint() ??
                       throw new InvalidOperationException($"Routing Endpoint wasn't set for the current request.");
        
        var canary = endpoint.Metadata.GetMetadata<CanaryModel>()
                    ?? throw new InvalidOperationException($"Routing Endpoint is missing {typeof(CanaryModel).FullName} metadata.");
        
        var circuitBreaker = endpoint.Metadata.GetMetadata<CircuitBreakerModel>()
                             ?? throw new InvalidOperationException($"Routing Endpoint is missing {typeof(CircuitBreakerModel).FullName} metadata.");
        
        var rateLimiter = endpoint.Metadata.GetMetadata<RateLimiterModel>()
                             ?? throw new InvalidOperationException($"Routing Endpoint is missing {typeof(RateLimiterModel).FullName} metadata.");


        context.Features.Set<IProxyFeature>(new ProxyFeature
        {
            Canary = canary,
            CircuitBreaker = circuitBreaker,
            RateLimiter = rateLimiter
        });

        return _next(context);
    }
}