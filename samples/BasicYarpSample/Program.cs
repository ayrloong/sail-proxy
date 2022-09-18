var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
var app = builder.Build();

app.UseRouting();
// Register the reverse proxy routes
app.UseEndpoints(endpoints =>
{
    endpoints.MapReverseProxy();
});
app.Run();