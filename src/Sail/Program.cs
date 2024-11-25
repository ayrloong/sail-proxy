using Sail.Core.Management;
using Sail.EntityFramework.Storage.Management;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

var apiVersioning = builder.Services.AddApiVersioning();

builder.AddServiceDefaults();

builder.AddDefaultOpenApi(apiVersioning);
builder.AddApplicationServices();
builder.Services.AddSailCore()
    .AddDatabaseStore();


var app = builder.Build();

app.MapSailApiService();
app.UseDefaultOpenApi();

app.Run();

