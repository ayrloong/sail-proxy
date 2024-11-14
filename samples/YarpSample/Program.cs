using Yarp.ReverseProxy.Configuration.ConfigProvider;
using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Model;
using YarpSample;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor(); // 注册 IHttpContextAccessor

builder.Services.AddTransient<CustomDelegatingHandler>();
builder.Services.AddSingleton<IForwarderHttpClientFactory, CustomForwarderHttpClientFactory>();
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));


var app = builder.Build();
app.MapReverseProxy();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    // We can customize the proxy pipeline and add/remove/replace steps
    endpoints.MapReverseProxy(proxyPipeline =>
    {
        // Use a custom proxy middleware, defined below
        proxyPipeline.Use((context, next) =>
        {
            var proxyFeature = context.Features.Get<IReverseProxyFeature>();
           // var user = proxyFeature.Route.Config.Extensions[typeof(UserModel)] as UserModel;
            
            //var abTest = proxyFeature.Route.Config.Extensions[typeof(ABTest)] as ABTest;

           // Console.WriteLine(abTest.CE);
            return next();
        });
        proxyPipeline.UseSessionAffinity();
        proxyPipeline.UseLoadBalancing();
    });
});
app.Run();



public class UserModel
{
    public string Name { get; set; }
}

public class ABTest
{
    public int CE { get; set; }

    public List<string> Names { get; set; }
}