using Sail.Kubernetes.Protocol.Configuration;

namespace Sail.Kubernetes.Protocol.Plugins;

public interface IPluginUpdater
{
    Task UpdateAsync(IEnumerable<PluginConfig> configs);
}