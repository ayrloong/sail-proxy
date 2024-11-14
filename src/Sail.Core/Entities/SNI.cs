namespace Sail.Core.Entities;

public class SNI
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string HostName { get; set; }
    public Guid CertificateId { get; set; }
    public Certificate Certificate { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}