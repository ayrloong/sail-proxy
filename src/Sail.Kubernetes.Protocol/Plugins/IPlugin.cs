using Sail.Kubernetes.Protocol.Configuration;

namespace Sail.Kubernetes.Protocol.Plugins;

public interface IPlugin
{
    Task ApplyAsync(IEnumerable<PluginConfig> plugins);
}