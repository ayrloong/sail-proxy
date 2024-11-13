using ErrorOr;
using Sail.Protocol.Apis;

namespace Sail.Protocol.Services;

public interface IClusterService
{
    Task<IEnumerable<ClusterVm>> GetAsync();
    Task<ErrorOr<Created>> CreateAsync(ClusterRequest request);
    Task<ErrorOr<Updated>> UpdateAsync(Guid id, ClusterRequest request);
    Task<ErrorOr<Deleted>> DeleteAsync(Guid id);
}