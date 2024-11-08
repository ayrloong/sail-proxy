using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Connections;

namespace Sail.Core.Certificates;

public interface IServerCertificateSelector
{
    X509Certificate2 GetCertificate(ConnectionContext connectionContext, string domainName);

    Task UpdateAsync(IReadOnlyList<CertificateConfig> certificates, CancellationToken cancellationToken);
}