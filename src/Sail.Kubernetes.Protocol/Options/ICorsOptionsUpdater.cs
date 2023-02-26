namespace Sail.Kubernetes.Protocol.Options;

public interface ICorsOptionsUpdater
{
    Task UpdateAsync(string name, List<string> allowOrigins, List<string> allowMethods, List<string> allowHeaders);
}