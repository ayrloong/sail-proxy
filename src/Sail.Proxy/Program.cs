using Serilog;
using Sail.Kubernetes.Protocol;
using Serilog.Sinks.SystemConsole.Themes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

using var serilog = new LoggerConfiguration()
               .MinimumLevel.Debug()
               .Enrich.FromLogContext()
               .WriteTo.Console(theme: AnsiConsoleTheme.Code)
               .CreateLogger();

builder.WebHost.ConfigureLogging(options =>
{
    options.ClearProviders();
    options.AddSerilog(serilog, dispose: false);
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("JwtBearer", builder => builder.RequireAuthenticatedUser());
    options.DefaultPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
});

var jwtBearerOptions = builder.Configuration.GetSection("JwtBearer").Get<Sail.Proxy.JwtBearerOptions>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = jwtBearerOptions.Authority;
        options.RequireHttpsMetadata = jwtBearerOptions.UseHttps;
        options.Audience = jwtBearerOptions.Audience;
    });

builder.Services.Configure<ReceiverOptions>(builder.Configuration);
builder.Services.AddHostedService<Receiver>();
builder.Services.AddReverseProxy().LoadFromMessages();

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapReverseProxy();
});

app.Run();