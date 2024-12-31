namespace Sail.Core.Entities;

public class RouteHeader
{
    public Guid Id { get; set; }
    public Guid MatchId { get; set; }
    public RouteMatch Match { get; set; }
    public string Name { get; set; }
    public List<string> Values { get; set; }
    public HeaderMatchMode Mode { get; set; }
    public bool IsCaseSensitive { get; set; }
    
}