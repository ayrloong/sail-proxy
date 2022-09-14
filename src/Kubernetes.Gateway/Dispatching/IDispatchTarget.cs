namespace Sail.Kubernetes.Gateway.Dispatching;

public interface IDispatchTarget
{
    public Task SendAsync(byte[] utf8Bytes, CancellationToken cancellationToken);
}