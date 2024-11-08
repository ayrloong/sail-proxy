using System.Collections.Concurrent;
using Polly;

namespace Yarp.Extensions.Resilience;

public class DefaultResiliencePipelineProvider : IResiliencePipelineProvider<HttpResponseMessage>
{
    private readonly ConcurrentDictionary<string, ResiliencePipeline> pipelines;
    public ResiliencePipeline<HttpResponseMessage> GetResiliencePipeline(string name)
    {
        return null;
    }
}