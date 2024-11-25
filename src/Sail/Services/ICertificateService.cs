using ErrorOr;
using Sail.Apis;

namespace Sail.Services;

public interface ICertificateService
{
    Task<IEnumerable<CertificateVm>> GetAsync();
    Task<ErrorOr<Created>> CreateAsync(CertificateRequest request);
    Task<ErrorOr<Updated>> UpdateAsync(Guid id, CertificateRequest request);
    Task<ErrorOr<Deleted>> DeleteAsync(Guid id);

    Task<IEnumerable<SNIVm>> GetSNIsAsync(Guid certificateId);
    Task<ErrorOr<Created>> CreateSNIAsync(Guid certificateId, SNIRequest request);
    Task<ErrorOr<Updated>> UpdateSNIAsync(Guid certificateId, Guid id, SNIRequest request);
    Task<ErrorOr<Deleted>> DeleteSNIAsync(Guid certificateId, Guid id);
}