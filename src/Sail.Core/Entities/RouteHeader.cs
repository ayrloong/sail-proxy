using RouteHeaderConfig = Yarp.ReverseProxy.Configuration.RouteHeader;
using HeaderMatchModeConfig = Yarp.ReverseProxy.Configuration.HeaderMatchMode;

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

    public RouteHeaderConfig ToRouteHeader()
    {
        return new RouteHeaderConfig
        {
            Name = Name,
            Values = Values,
            Mode = (HeaderMatchModeConfig)Mode,
            IsCaseSensitive = IsCaseSensitive
        };
    }
}