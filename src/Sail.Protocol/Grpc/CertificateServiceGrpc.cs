using Grpc.Core;

namespace Sail.Protocol.Grpc;

public class CertificateServiceGrpc : CertificateService.CertificateServiceBase
{
    public override Task<CreateCertificateResponse> Create(Certificate request, ServerCallContext context)
    {
        throw new NotImplementedException();
    }
}