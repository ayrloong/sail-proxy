using Sail.Core.Management;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

builder.Services.AddSailCore();

app.Run();