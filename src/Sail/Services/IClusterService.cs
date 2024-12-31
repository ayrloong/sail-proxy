using ErrorOr;
using Sail.Apis;

namespace Sail.Services;

public interface IClusterService
{
    Task<IEnumerable<ClusterVm>> GetAsync();
    Task<ErrorOr<Created>> CreateAsync(ClusterRequest request);
    Task<ErrorOr<Updated>> UpdateAsync(Guid id, ClusterRequest request);
    Task<ErrorOr<Deleted>> DeleteAsync(Guid id);
}