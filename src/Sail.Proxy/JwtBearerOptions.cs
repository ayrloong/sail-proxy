namespace Sail.Proxy;

public class JwtBearerOptions
{
    public string Authority { get; set; }
    public bool UseHttps { get; set; }
    public string Audience { get; set; }
}