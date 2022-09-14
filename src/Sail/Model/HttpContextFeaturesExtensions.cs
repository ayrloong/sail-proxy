using Microsoft.AspNetCore.Http;
using Yarp.ReverseProxy.Model;

namespace Sail.Model;

public static class HttpContextFeaturesExtensions
{
    public static IProxyFeature GetProxyFeature(this HttpContext context)
    {
        return context.Features.Get<IProxyFeature>() ?? throw new InvalidOperationException($"{typeof(IProxyFeature).FullName} is missing.");
    }

}