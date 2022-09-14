using System.Text.Json;
using Microsoft.Kubernetes;
using Sail.Kubernetes.Controller.Caching;
using Sail.Kubernetes.Controller.Converters;
using Sail.Kubernetes.Controller.Dispatching;
using Sail.Kubernetes.Protocol;

namespace Sail.Kubernetes.Controller.Services;

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

    private void Attached(IDispatchTarget target)
    {
        _attached?.Invoke(target);
    }

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        try
        {
            var ingresses = _cache.GetIngresses().ToArray();

            var message = new Message
            {
                MessageType = MessageType.Update,
                Key = string.Empty,
            };

            var configContext = new SailConfigContext();

            foreach (var ingress in ingresses)
            {
                if (_cache.TryGetReconcileData(
                        new NamespacedName(ingress.Metadata.NamespaceProperty, ingress.Metadata.Name), out var data))
                {
                    var ingressContext = new SailIngressContext(ingress, data.ServiceList, data.EndpointsList);
                    SailParser.ConvertFromKubernetesIngress(ingressContext, configContext);
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
}