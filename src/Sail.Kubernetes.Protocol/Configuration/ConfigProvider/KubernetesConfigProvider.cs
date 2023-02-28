using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

namespace Sail.Kubernetes.Protocol.Configuration;

public class KubernetesConfigProvider : IProxyConfigProvider, IUpdateConfig
{
    private volatile MessageConfig _config;

    public KubernetesConfigProvider()
    {
        _config = new MessageConfig(null, null);
    }

    public IProxyConfig GetConfig() => _config;

    public async Task UpdateAsync(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
    {
        var oldConfig = _config;
        _config = new MessageConfig(routes, clusters);
        oldConfig.SignalChange();
    }

    private class MessageConfig : IProxyConfig
    {
        private readonly CancellationTokenSource _cts = new();

        public MessageConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters) :
            this(routes, clusters, Guid.NewGuid().ToString())
        {
        }

        public MessageConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters,
            string revisionId)
        {
            RevisionId = revisionId ?? throw new ArgumentNullException(nameof(revisionId));
            Routes = routes;
            Clusters = clusters;
            ChangeToken = new CancellationChangeToken(_cts.Token);
        }

        public string RevisionId { get; }
        public IReadOnlyList<RouteConfig> Routes { get; }
        public IReadOnlyList<ClusterConfig> Clusters { get; }
        public IChangeToken ChangeToken { get; }

        internal void SignalChange()
        {
            _cts.Cancel();
        }
    }
}