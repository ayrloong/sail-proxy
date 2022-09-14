using System.Text.Json;
using Microsoft.Kubernetes;
using Sail.Kubernetes.Gateway.Caching;
using Sail.Kubernetes.Gateway.Converters;
using Sail.Kubernetes.Gateway.Dispatching;
using Sail.Kubernetes.Protocol;

namespace Sail.Kubernetes.Gateway.Services;

public class Reconciler : IReconciler
{
    private readonly ICache _cache;
    private readonly IDispatcher _dispatcher;
    private Action<IDispatchTarget> _attached;
    private readonly ILogger<Reconciler> _logger;

    public Reconciler(ICache cache, IDispatcher dispatcher, ILogger<Reconciler> logger)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        _dispatcher.OnAttach(Attached);
        _logger = logger;
    }

    public void OnAttach(Action<IDispatchTarget> attached)
    {
        _attached = attached;
    }

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        try
        {
            var gateways = _cache.GetGatewayes().ToArray();
            var message = new Message
            {
                MessageType = MessageType.Update,
                Key = string.Empty,
            };

            var configContext = new SailConfigContext();

            foreach (var gateway in gateways)
            {
                if (_cache.TryGetReconcileData(new NamespacedName(gateway.Metadata.NamespaceProperty, gateway.Metadata.Name), out var data))
                {
                    var gatewayContext = new SailGatewayContext(gateway, data.ServiceList, data.EndpointsList);
                    SailParser.ConvertFromKubernetesGateway(gatewayContext, configContext);
                }
            }

            message.Cluster = configContext.BuildClusterConfig();
            message.Routes = configContext.Routes;

            var bytes = JsonSerializer.SerializeToUtf8Bytes(message);

            _logger.LogInformation(JsonSerializer.Serialize(message));

            await _dispatcher.SendAsync(null, bytes, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex.Message);
            throw;
        }
    }

    private void Attached(IDispatchTarget target)
    {
        _attached?.Invoke(target);
    }
}