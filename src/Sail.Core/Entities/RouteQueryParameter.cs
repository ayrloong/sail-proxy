using RouteQueryParameterConfig = Yarp.ReverseProxy.Configuration.RouteQueryParameter;
using QueryParameterMatchModeConfig = Yarp.ReverseProxy.Configuration.QueryParameterMatchMode;

namespace Sail.Core.Entities;

public class RouteQueryParameter
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<string> Values { get; set; }
    public QueryParameterMatchMode Mode { get; set; }
    public bool IsCaseSensitive { get; set; }
    public RouteQueryParameterConfig ToRouteQueryParameter()
    {
        return new RouteQueryParameterConfig
        {
            Name = Name,
            Values = Values,
            Mode = (QueryParameterMatchModeConfig)Mode,
            IsCaseSensitive = IsCaseSensitive
        };
    }
}