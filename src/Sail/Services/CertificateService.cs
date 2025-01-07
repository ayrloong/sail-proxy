using ErrorOr;
using Sail.Apis;
using Sail.Core.Entities;
using Sail.Storage.MongoDB;
using MongoDB.Driver;

namespace Sail.Services;

public class CertificateService(SailContext context) : ICertificateService
{
    public async Task<IEnumerable<CertificateVm>> GetAsync(CancellationToken cancellationToken = default)
    {

        var filter = Builders<Certificate>.Filter.Empty;
        var certificates = await context.Certificates.FindAsync(filter, cancellationToken: cancellationToken);
        var items = await certificates.ToListAsync(cancellationToken: cancellationToken);
        return items.Select(MapToCertificateVm);
    }

    public async Task<ErrorOr<Created>> CreateAsync(CertificateRequest request,
        CancellationToken cancellationToken = default)
    {
        var certificate = new Certificate
        {
            Cert = request.Cert,
            Key = request.Key
        };

        await context.Certificates.InsertOneAsync(certificate, cancellationToken: cancellationToken);
        return Result.Created;
    }

    public async Task<ErrorOr<Updated>> UpdateAsync(Guid id, CertificateRequest request,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<Certificate>.Filter.And(Builders<Certificate>.Filter.Where(x => x.Id == id));

        var update = Builders<Certificate>.Update
            .Set(x => x.Cert, request.Cert)
            .Set(x => x.Key, request.Key)
            .Set(x => x.UpdatedAt, DateTimeOffset.UtcNow);

        await context.Certificates.FindOneAndUpdateAsync(filter, update, cancellationToken: cancellationToken);
        return Result.Updated;
    }

    public async Task<ErrorOr<Deleted>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Certificate>.Filter.And(Builders<Certificate>.Filter.Where(x => x.Id == id));
        await context.Certificates.DeleteOneAsync(filter, cancellationToken);
        return Result.Deleted;
    }

    public async Task<IEnumerable<SNIVm>> GetSNIsAsync(Guid certificateId,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<Certificate>.Filter.And(Builders<Certificate>.Filter.Where(x => x.Id == certificateId));
        var items = await context.Certificates.FindAsync(filter, cancellationToken: cancellationToken);
        var certificate = await items.SingleOrDefaultAsync(cancellationToken: cancellationToken);
        if (certificate is null)
        {
            return [];
        }

        return certificate.SNIs.Select(MapToSNIVm);
    }

    public async Task<ErrorOr<Created>> CreateSNIAsync(Guid certificateId, SNIRequest request,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<Certificate>.Filter.And(Builders<Certificate>.Filter.Where(x => x.Id == certificateId));

        var sni = new SNI
        {
            Name = request.Name,
            HostName = request.HostName
        };
        var update = Builders<Certificate>.Update.Push(x => x.SNIs, sni);
        await context.Certificates.FindOneAndUpdateAsync(filter, update, cancellationToken: cancellationToken);
        return Result.Created;
    }

    public async Task<ErrorOr<Updated>> UpdateSNIAsync(Guid certificateId, Guid id, SNIRequest request,
        CancellationToken cancellationToken = default)
    {
        var filter = Builders<Certificate>.Filter.And(Builders<Certificate>.Filter.Where(x => x.Id == certificateId),
            Builders<Certificate>.Filter.ElemMatch(x => x.SNIs, f => f.Id == id));

        var update = Builders<Certificate>.Update.Set(x => x.SNIs[-1], new SNI());
        await context.Certificates.FindOneAndUpdateAsync(filter, update, cancellationToken: cancellationToken);
        return Result.Updated;
    }

    public async Task<ErrorOr<Deleted>> DeleteSNIAsync(Guid certificateId, Guid id,
        CancellationToken cancellationToken = default)
    {

        var filter = Builders<Certificate>.Filter.And(Builders<Certificate>.Filter.Where(x => x.Id == certificateId),
            Builders<Certificate>.Filter.ElemMatch(x => x.SNIs, f => f.Id == id));

        await context.Certificates.DeleteOneAsync(filter, cancellationToken: cancellationToken);
        return Result.Deleted;
    }

    private CertificateVm MapToCertificateVm(Certificate certificate)
    {
        return new CertificateVm
        {
            Id = certificate.Id,
            Cert = certificate.Cert,
            Key = certificate.Key,
            CreatedAt = certificate.CreatedAt,
            UpdatedAt = certificate.UpdatedAt
        };
    }

    private SNIVm MapToSNIVm(SNI sni)
    {
        return new SNIVm
        {
            Id = sni.Id,
            Name = sni.Name,
            HostName = sni.HostName,
            CreatedAt = sni.CreatedAt,
            UpdatedAt = sni.UpdatedAt
        };
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