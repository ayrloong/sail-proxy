using Sail.Configuration;

namespace Sail.Model;

public class RateLimiterModel
{
    public RateLimiterModel(RateLimiterConfig config)
    {
        Config = config;
    }
    public RateLimiterConfig Config { get; }
}