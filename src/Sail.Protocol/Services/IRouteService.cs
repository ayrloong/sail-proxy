namespace Sail.Protocol.Services;

public interface IRouteService
{
    Task<IEnumerable<Core.Entities.Route>> GetAsync();
}