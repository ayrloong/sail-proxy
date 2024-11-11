namespace Sail.Core.Entities;

public class Sni
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string HostName { get; set; }
    public Guid CertificateId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}