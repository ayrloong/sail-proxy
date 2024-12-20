using System.Collections.Concurrent;
using Polly;
using Polly.CircuitBreaker;
using Polly.Registry;
using Polly.Retry;

namespace Yarp.Extensions.Resilience;

public class DefaultResiliencePipelineProvider : IResiliencePipelineProvider<HttpResponseMessage>,
    IResiliencePipelineUpdateConfig
{
    
    public ResiliencePipeline<HttpResponseMessage> GetResiliencePipeline(string name)
    {
        throw new Exception();
    }

    private ResiliencePipelineBuilder<HttpResponseMessage> ConfigureCircuitBreaker(
        ResiliencePipelineBuilder<HttpResponseMessage> builder, CircuitBreakerConfig config)
    {
        var strategyOptions = new CircuitBreakerStrategyOptions<HttpResponseMessage>();
        
        return builder.AddCircuitBreaker(strategyOptions);
    }

    private ResiliencePipelineBuilder<HttpResponseMessage> ConfigureRetry(
        ResiliencePipelineBuilder<HttpResponseMessage> builder, RetryConfig config)
    {
        var strategyOptions = new RetryStrategyOptions<HttpResponseMessage>();

        return builder.AddRetry(strategyOptions);
    }

    public ValueTask UpdateAsync()
    {
        throw new NotImplementedException();
    }
}