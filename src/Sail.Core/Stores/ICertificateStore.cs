using Sail.Core.Entities;

namespace Sail.Core.Stores;

public interface ICertificateStore
{
    Task<List<Certificate>> GetAsync(CancellationToken cancellationToken);
}