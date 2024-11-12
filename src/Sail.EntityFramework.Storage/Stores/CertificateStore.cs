using Microsoft.EntityFrameworkCore;
using Sail.Core.Entities;
using Sail.Core.Stores;

namespace Sail.EntityFramework.Storage.Stores;

public class CertificateStore(ConfigurationContext context) : ICertificateStore
{
    public Task<List<Certificate>> GetAsync(CancellationToken cancellationToken)
    {
        return context.Certificates.ToListAsync(cancellationToken: cancellationToken);
    }
}