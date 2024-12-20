using Sail.Api.V1;

namespace Sail.Compass.Caching;

public interface ICache
{
    ValueTask UpdateAsync(IReadOnlyList<Route> routes, CancellationToken cancellationToken = default);
}