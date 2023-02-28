namespace Sail.Kubernetes.Protocol.Configuration;

public class MiddlewareUpdater : IMiddlewareUpdater
{
    private readonly IAuthenticationSchemeUpdater _authenticationSchemeUpdater;
    private readonly ICorsOptionsUpdater _corsOptionsUpdater;
    private readonly IRateLimiterOptionsUpdater _rateLimiterOptionsUpdater;

    public MiddlewareUpdater(IAuthenticationSchemeUpdater authenticationSchemeUpdater,
        ICorsOptionsUpdater corsOptionsUpdater, IRateLimiterOptionsUpdater rateLimiterOptionsUpdater)
    {
        _authenticationSchemeUpdater = authenticationSchemeUpdater;
        _corsOptionsUpdater = corsOptionsUpdater;
        _rateLimiterOptionsUpdater = rateLimiterOptionsUpdater;
    }

    public async Task UpdateAsync(IReadOnlyList<MiddlewareConfig> middlewares)
    {
        foreach (var middleware in middlewares)
        {
            if (middleware.JwtBearer is not null)
            {
                var jwtBearer = middleware.JwtBearer;
                await _authenticationSchemeUpdater.UpdateAsync(jwtBearer);
            }

            if (middleware.Cors is not null)
            {
                var cors = middleware.Cors;
                await _corsOptionsUpdater.UpdateAsync(cors);
            }

            if (middleware.RateLimiter is not null)
            {
                var rateLimiter = middleware.RateLimiter;
                await _rateLimiterOptionsUpdater.UpdateAsync(rateLimiter);
            }
        }
    }
}