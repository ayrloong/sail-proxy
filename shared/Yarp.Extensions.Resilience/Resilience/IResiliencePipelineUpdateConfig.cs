namespace Yarp.Extensions.Resilience;

public interface IResiliencePipelineUpdateConfig
{
    ValueTask UpdateAsync();
}