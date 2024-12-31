
using Sail.Api.V1;
using Sail.Compass.Management;
using Sail.Core.Management;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSailCore();

builder.Services.AddControllerRuntime();
builder.Services.AddReverseProxy().LoadFromMessages();
builder.Services.AddGrpcClient<ClusterService.ClusterServiceClient>(o => o.Address = new Uri("http://localhost:8000"));
builder.Services.AddGrpcClient<RouteService.RouteServiceClient>(o => o.Address = new Uri("http://localhost:8000"));


var app = builder.Build();
app.MapReverseProxy();
app.Run();