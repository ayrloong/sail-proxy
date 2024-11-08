using Microsoft.Extensions.Hosting;
using Sail.Core.Certificates;
using Sail.Core.Converters;
using Sail.Core.Stores;

namespace Sail.Core.Hosting;

public class CertificateHostedService(
    IServerCertificateSelector serverCertificateSelector,
    ICertificateStore certificateStore) : IHostedService, IDisposable
{
    private readonly PeriodicTimer? _timer = new(TimeSpan.FromSeconds(5));

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (await _timer!.WaitForNextTickAsync(cancellationToken) && !cancellationToken.IsCancellationRequested)
        {
            await UpdateAsync(cancellationToken);
        }
    }

    private async Task UpdateAsync(CancellationToken cancellationToken)
    {
        try
        {
            var certificates = await certificateStore.GetAsync(cancellationToken);
            var config = Parser.ConvertCertificates(certificates);

            await serverCertificateSelector.UpdateAsync(config, cancellationToken);

        }
        catch (Exception e)
        {
            // ignored
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _timer!.WaitForNextTickAsync(cancellationToken);
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}