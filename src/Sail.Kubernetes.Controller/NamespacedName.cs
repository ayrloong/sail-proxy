using System.Diagnostics.CodeAnalysis;
using k8s;
using k8s.Models;
using Newtonsoft.Json;

namespace Sail.Kubernetes.Controller;

public struct NamespacedName : IEquatable<NamespacedName>
{

    [JsonConstructor]
    public NamespacedName(string @namespace, string name)
    {
        Namespace = @namespace;
        Name = name;
    }

    public NamespacedName(string name)
    {
        Namespace = null;
        Name = name;
    }


    public string Namespace { get; }

    public string Name { get; }

    public static bool operator ==(NamespacedName left, NamespacedName right)
    {
        return left.Equals(right);
    }


    public static bool operator !=(NamespacedName left, NamespacedName right)
    {
        return !(left == right);
    }

    public static NamespacedName From(IKubernetesObject<V1ObjectMeta> resource)
    {
        if (resource is null)
        {
            throw new ArgumentNullException(nameof(resource));
        }

        return new NamespacedName(resource.Namespace(), resource.Name());
    }


    public static NamespacedName From(V1ObjectMeta metadata, [NotNull] V1OwnerReference ownerReference,
        bool? clusterScoped = null)
    {
        _ = metadata ?? throw new ArgumentNullException(nameof(metadata));
        _ = ownerReference ?? throw new ArgumentNullException(nameof(ownerReference));

        return new NamespacedName(
            clusterScoped ?? false ? null : metadata.NamespaceProperty,
            ownerReference.Name);
    }

    public override bool Equals(object obj)
    {
        return obj is NamespacedName name && Equals(name);
    }

    public bool Equals([AllowNull] NamespacedName other)
    {
        return Namespace == other.Namespace && Name == other.Name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Namespace, Name);
    }

    public override string ToString()
    {
        return $"{Namespace}/{Name}";
    }
}