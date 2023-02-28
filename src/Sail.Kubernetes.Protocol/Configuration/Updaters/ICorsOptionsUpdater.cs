namespace Sail.Kubernetes.Protocol.Configuration;

public interface ICorsOptionsUpdater
{
    Task UpdateAsync(CorsConfig cors);
}