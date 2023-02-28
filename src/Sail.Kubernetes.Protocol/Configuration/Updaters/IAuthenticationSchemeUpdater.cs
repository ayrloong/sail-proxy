namespace Sail.Kubernetes.Protocol.Configuration;

public interface IAuthenticationSchemeUpdater
{
    Task UpdateAsync(JwtBearerConfig jwtBearer);
}