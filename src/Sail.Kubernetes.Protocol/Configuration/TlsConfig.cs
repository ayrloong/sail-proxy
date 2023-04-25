namespace Sail.Kubernetes.Protocol.Configuration;

public struct TlsConfig
{
    public string HostName { get; set; }
    public string Cert { get; set; }
    public string PrivateKey { get; set; }
}