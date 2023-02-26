namespace Sail.Kubernetes.Protocol.Configuration;

public class CorsConfig
{
    public string Name { get; set; }
    public List<string> AllowOrigins { get; set; }
    public List<string> AllowMethods { get; set; }
    public List<string> AllowHeaders { get; set; }
}