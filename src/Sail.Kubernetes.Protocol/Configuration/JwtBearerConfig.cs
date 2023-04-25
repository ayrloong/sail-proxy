namespace Sail.Kubernetes.Protocol.Configuration;

public class JwtBearerConfig
{
    public string Name { get; set; }
    public string Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string OpenIdConfiguration { get; set; }
}