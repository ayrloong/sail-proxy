using ErrorOr;
using Sail.Apis;

namespace Sail.Services;

public interface IClusterService
{
    Task<IEnumerable<ClusterVm>> GetAsync(CancellationToken cancellationToken = default);
    Task<ErrorOr<Created>> CreateAsync(ClusterRequest request, CancellationToken cancellationToken = default);
    Task<ErrorOr<Updated>> UpdateAsync(Guid id, ClusterRequest request,CancellationToken cancellationToken = default);
    Task<ErrorOr<Deleted>> DeleteAsync(Guid id,CancellationToken cancellationToken = default);
}