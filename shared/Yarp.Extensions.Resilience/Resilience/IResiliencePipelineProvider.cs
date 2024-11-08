using Polly;

namespace Yarp.Extensions.Resilience;

public interface IResiliencePipelineProvider<TResult> where TResult : IDisposable
{
    ResiliencePipeline<TResult> GetResiliencePipeline(string name);
}