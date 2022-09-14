using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

namespace Sail.Kubernetes.Protocol;
public class MessageConfigProvider : IProxyConfigProvider, IUpdateConfig
{
    private volatile MessageConfig _config;
    public MessageConfigProvider()
    {
        _config = new MessageConfig(null, null);
    }

    public IProxyConfig GetConfig() => _config;

    public void Update(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
    {
        var oldConfig = _config;
        _config = new MessageConfig(routes, clusters);
        oldConfig.SignalChange();
    }
    private class MessageConfig : IProxyConfig
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public MessageConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
        {
            Routes = routes;
            Clusters = clusters;
            ChangeToken = new CancellationChangeToken(_cts.Token);
        }

        public IReadOnlyList<RouteConfig> Routes { get; }

        public IReadOnlyList<ClusterConfig> Clusters { get; }

        public IChangeToken ChangeToken { get; }

        internal void SignalChange()
        {
            _cts.Cancel();
        }
    }
}
