using Sail.Configuration;

namespace Sail.Model;

public class CanaryModel
{
    public CanaryModel(CanaryConfig config)
    {
        Config = config;
    }
    public CanaryConfig Config { get; }
    
}