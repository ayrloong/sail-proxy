using Sail.Kubernetes.Protocol.Configuration;

namespace Sail.Kubernetes.Protocol.Plugins;

public class PluginUpdater : IPluginUpdater
{
    private readonly IEnumerable<IPlugin> _plugins;

    public PluginUpdater(IEnumerable<IPlugin> plugins)
    {
        _plugins = plugins;
    }
    
    public Task UpdateAsync(IEnumerable<PluginConfig> configs)
    {
        foreach (var plugin in _plugins)
        {
            plugin.ApplyAsync(configs);
        }

        return Task.CompletedTask;
    }
}