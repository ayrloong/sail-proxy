using ErrorOr;
using Sail.Core.Entities;

namespace Sail.Protocol.Services;

public interface IClusterService
{
    Task<IEnumerable<Cluster>> GetAsync();
    Task<ErrorOr<Created>> CreateAsync();
    Task<ErrorOr<Updated>> UpdateAsync();
    Task<ErrorOr<Deleted>> DeleteAsync(Guid id);
}