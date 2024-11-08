using Sail.Core.Management;
using Sail.EntityFramework.Storage.Management;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddDefaultOpenApi();
builder.AddApplicationServices();

builder.Services.AddProblemDetails();

builder.Services.AddSailCore()
    .AddDatabaseStore();

var app = builder.Build();
 
app.MapSailGrpcService();
app.MapSailApiService();

app.UseDefaultOpenApi();

app.MapReverseProxy();
app.Run();

