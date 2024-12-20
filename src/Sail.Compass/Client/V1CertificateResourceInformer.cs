using Grpc.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sail.Api.V1;

namespace Sail.Compass.Client;

public class V1CertificateResourceInformer(
    IHostApplicationLifetime hostApplicationLifetime,
    CertificateService.CertificateServiceClient client,
    ILogger logger) : ResourceInformer<CertificateItems>(hostApplicationLifetime, logger)
{
    protected override Task<IAsyncStreamReader<CertificateItems>> RetrieveResourceAsync(
        CancellationToken cancellationToken = default)
    {
        var response = client.StreamCertificates(new CertificateRequest());
        return Task.FromResult(response.ResponseStream);
    }
}