using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Kubernetes.Controller.Hosting;
using Microsoft.Kubernetes.Controller.Rate;
using Sail.Kubernetes.Protocol.Configuration;
using Sail.Kubernetes.Protocol.Middlewares;

namespace Sail.Kubernetes.Protocol;

public class Receiver : BackgroundHostedService
{
    private readonly ReceiverOptions _options;
    private readonly Limiter _limiter;
    private readonly IUpdateConfig _proxyConfigProvider;
    private readonly IMiddlewareUpdater _middlewareUpdater;

    public Receiver(
        IOptions<ReceiverOptions> options,
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger<Receiver> logger,
        IUpdateConfig proxyConfigProvider,
        IMiddlewareUpdater middlewareUpdater) : base(hostApplicationLifetime, logger)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        _options = options.Value;
        _limiter = new Limiter(new Limit(2), 3);
        _proxyConfigProvider = proxyConfigProvider;
        _middlewareUpdater = middlewareUpdater;
    }

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        using var client = new HttpClient();
        while (!cancellationToken.IsCancellationRequested)
        {
            await _limiter.WaitAsync(cancellationToken).ConfigureAwait(false);
            Logger.LogInformation("Connecting with {ControllerUrl}", _options.ControllerUrl.ToString());

            try
            {
                await using var stream = await client.GetStreamAsync(_options.ControllerUrl, cancellationToken)
                    .ConfigureAwait(false);
                using var reader = new StreamReader(stream, Encoding.UTF8, leaveOpen: true);
                await using var cancellation = cancellationToken.Register(stream.Close);
                while (true)
                {
                    var json = await reader.ReadLineAsync(cancellationToken).ConfigureAwait(false);
                    if (string.IsNullOrEmpty(json))
                    {
                        break;
                    }

                    var message = JsonSerializer.Deserialize<Message>(json);
                    Logger.LogInformation("Received {MessageType} for {MessageKey}", message.MessageType, message.Key);

                    if (message.MessageType == MessageType.Update)
                    {
                        await _middlewareUpdater.UpdateAsync(message.Middlewares);
                        await _proxyConfigProvider.UpdateAsync(message.Routes, message.Cluster).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogInformation("Stream ended: {Message}", ex.Message);
            }
        }
    }
}