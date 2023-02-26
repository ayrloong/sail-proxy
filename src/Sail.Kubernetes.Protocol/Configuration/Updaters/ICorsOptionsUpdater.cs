namespace Sail.Kubernetes.Protocol.Configuration;

public interface ICorsOptionsUpdater
{
    Task UpdateAsync(string name, List<string> allowOrigins, List<string> allowMethods, List<string> allowHeaders);
}