using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Connections;

namespace Sail.Kubernetes.Protocol.Certificates;

public class ServerCertificateSelector : IServerCertificateSelector
{
    public X509Certificate2 GetCertificate(ConnectionContext connectionContext, string domainName)
    {
        throw new NotImplementedException();
    }
}