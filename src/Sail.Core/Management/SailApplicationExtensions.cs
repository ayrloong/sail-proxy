using Microsoft.Extensions.DependencyInjection;
using Sail.Core.Configuration.ConfigProvider;
using Sail.Core.Hosting;
using Yarp.ReverseProxy.Configuration;

namespace Sail.Core.Management;

public static class SailApplicationExtensions
{
    public static SailApplication AddSailCore(this IServiceCollection services)
    {
        var app = new SailApplication(services);
        return app;
    }
}