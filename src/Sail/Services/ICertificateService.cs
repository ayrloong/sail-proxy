using ErrorOr;
using Sail.Apis;

namespace Sail.Services;

public interface ICertificateService
{
    Task<IEnumerable<CertificateVm>> GetAsync(CancellationToken cancellationToken = default);
    Task<ErrorOr<Created>> CreateAsync(CertificateRequest request, CancellationToken cancellationToken = default);
    Task<ErrorOr<Updated>> UpdateAsync(Guid id, CertificateRequest request,
        CancellationToken cancellationToken = default);
    Task<ErrorOr<Deleted>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SNIVm>> GetSNIsAsync(Guid certificateId, CancellationToken cancellationToken = default);
    Task<ErrorOr<Created>> CreateSNIAsync(Guid certificateId, SNIRequest request,
        CancellationToken cancellationToken = default);
    Task<ErrorOr<Updated>> UpdateSNIAsync(Guid certificateId, Guid id, SNIRequest request,
        CancellationToken cancellationToken = default);

    Task<ErrorOr<Deleted>> DeleteSNIAsync(Guid certificateId, Guid id, CancellationToken cancellationToken = default);
}