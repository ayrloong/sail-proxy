using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Sail.Core.Entities;
using Sail.EntityFramework.Storage;

namespace Sail.Protocol.Services;

public class CertificateService(ConfigurationContext context) : ICertificateService
{
    public async Task<IEnumerable<Certificate>> GetAsync()
    {
        return await context.Certificates.ToListAsync();
    }

    public async Task<ErrorOr<Created>> CreateAsync()
    {
        var certificate = new Certificate();
        await context.Certificates.AddAsync(certificate);
        await context.SaveChangesAsync();
        return Result.Created;
    }

    public async Task<ErrorOr<Updated>> UpdateAsync()
    {
        var certificate = new Certificate();
        context.Certificates.Update(certificate);
        await context.SaveChangesAsync();
        return Result.Updated;
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(Guid id)
    {
        var certificate = new Certificate();
        context.Certificates.Remove(certificate);
        await context.SaveChangesAsync();
        return Result.Deleted;
    }
}