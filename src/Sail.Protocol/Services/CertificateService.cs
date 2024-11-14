using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Sail.Core.Entities;
using Sail.EntityFramework.Storage;
using Sail.Protocol.Apis;

namespace Sail.Protocol.Services;

public class CertificateService(ConfigurationContext context) : ICertificateService
{
    public async Task<IEnumerable<CertificateVm>> GetAsync()
    {
        var items = await context.Certificates.Include(certificate => certificate.SNIs).ToListAsync();

        return items.Select(x => new CertificateVm
        {
            Id = x.Id,
            Cert = x.Cert,
            Key = x.Key,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        });
    }

    public async Task<ErrorOr<Created>> CreateAsync(CertificateRequest request)
    {
        var certificate = new Certificate
        {
            Cert = request.Cert,
            Key = request.Key
        };
        await context.Certificates.AddAsync(certificate);
        await context.SaveChangesAsync();
        return Result.Created;
    }

    public async Task<ErrorOr<Updated>> UpdateAsync(Guid id, CertificateRequest request)
    {
        var certificate = await context.Certificates.FindAsync(id);
        if (certificate is null)
        {
            return Error.NotFound(description: "Certificate not found");
        }

        certificate.Cert = request.Cert;
        certificate.Key = request.Key;
        context.Certificates.Update(certificate);
        await context.SaveChangesAsync();
        return Result.Updated;
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(Guid id)
    {
        var certificate = await context.Certificates.FindAsync(id);
        if (certificate is null)
        {
            return Error.NotFound(description: "Certificate not found");
        }

        context.Certificates.Remove(certificate);
        await context.SaveChangesAsync();
        return Result.Deleted;
    }

    public async Task<IEnumerable<SNIVm>> GetSNIsAsync(Guid certificateId)
    {
        var certificate = await context.Certificates.Include(certificate => certificate.SNIs)
            .SingleOrDefaultAsync(x => x.Id == certificateId);

        if (certificate is null)
        {
            return Enumerable.Empty<SNIVm>();
        }

        return certificate.SNIs.Select(x => new SNIVm
        {
            Id = x.Id,
            Name = x.Name,
            HostName = x.HostName,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt
        });
    }

    public async Task<ErrorOr<Created>> CreateSNIAsync(Guid certificateId, SNIRequest request)
    {
        var certificate = await context.Certificates.Include(certificate => certificate.SNIs)
            .SingleOrDefaultAsync(x => x.Id == certificateId);
        if (certificate is null)
        {
            return Error.NotFound(description: "Certificate not found");
        }

        certificate.SNIs.Add(new SNI
        {
            Name = request.Name,
            HostName = request.HostName
        });
        context.Certificates.Update(certificate);
        await context.SaveChangesAsync();
        return Result.Created;
    }

    public async Task<ErrorOr<Updated>> UpdateSNIAsync(Guid certificateId, Guid id, SNIRequest request)
    {
        var certificate = await context.Certificates.Include(certificate => certificate.SNIs)
            .SingleOrDefaultAsync(x => x.Id == certificateId);

        if (certificate is null)
        {
            return Error.NotFound(description: "Certificate not found");
        }

        var sni = certificate.SNIs.SingleOrDefault(x => x.Id == id);
        if (sni is null)
        {
            return Error.NotFound(description: "SNI not found");
        }

        sni.Name = request.Name;
        sni.HostName = request.HostName;
        sni.UpdatedAt = DateTimeOffset.Now;

        context.Certificates.Update(certificate);
        await context.SaveChangesAsync();
        return Result.Updated;
    }

    public async Task<ErrorOr<Deleted>> DeleteSNIAsync(Guid certificateId, Guid id)
    {
        var certificate = await context.Certificates.Include(certificate => certificate.SNIs)
            .SingleOrDefaultAsync(x => x.Id == certificateId);

        if (certificate is null)
        {
            return Error.NotFound(description: "Certificate not found");
        }

        var sni = certificate.SNIs.SingleOrDefault(x => x.Id == id);
        if (sni is null)
        {
            return Error.NotFound(description: "SNI not found");
        }

        certificate.SNIs.Remove(sni);
        context.Certificates.Update(certificate);
        await context.SaveChangesAsync();
        return Result.Deleted;
    }
}

public record CertificateVm
{
    public Guid Id { get; init; }
    public string Cert { get; init; }
    public string Key { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
}

public record SNIVm
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string HostName { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
}