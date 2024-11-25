using ErrorOr;
using Sail.Core.Entities;
using Sail.Apis;

namespace Sail.Services;

public interface IRouteService
{
    Task<IEnumerable<RouteVm>> GetAsync();
    Task<ErrorOr<Created>> CreateAsync(RouteRequest request);
    Task<ErrorOr<Updated>> UpdateAsync(Guid id, RouteRequest request);
    Task<ErrorOr<Deleted>> DeleteAsync(Guid id);
}