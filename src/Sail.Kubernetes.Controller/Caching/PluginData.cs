using k8s.Models;
using Sail.Kubernetes.Controller.Models;

namespace Sail.Kubernetes.Controller.Caching;

public struct PluginData
{
    public PluginData(V1beta1Plugin plugin)
    {
        if (plugin is null)
        {
            throw new ArgumentNullException(nameof(plugin));
        }

        Spec = plugin.Spec;
        Metadata = plugin.Metadata;
    }

    public V1beta1PluginSpec Spec { get; }
    public V1ObjectMeta Metadata { get; }
}