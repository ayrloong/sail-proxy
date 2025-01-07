using ErrorOr;
using Sail.Core.Entities;
using Sail.Apis;

namespace Sail.Services;

public interface IRouteService
{
    Task<IEnumerable<RouteVm>> GetAsync(CancellationToken cancellationToken = default);
    Task<ErrorOr<Created>> CreateAsync(RouteRequest request, CancellationToken cancellationToken = default);
    Task<ErrorOr<Updated>> UpdateAsync(Guid id, RouteRequest request, CancellationToken cancellationToken = default);
    Task<ErrorOr<Deleted>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}