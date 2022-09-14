using Sail.Configuration;

namespace Sail.Model;

public class CircuitBreakerModel
{
    public CircuitBreakerModel(CircuitBreakerConfig config)
    {
        Config = config;
    }
    public CircuitBreakerConfig Config { get; }
}