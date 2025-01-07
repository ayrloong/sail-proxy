using Sail.Apis;
using Sail.Core.Management;
using Sail.Storage.MongoDB.Management;
using Sail.Extensions;
using Sail.Grpc;
using Sail.Storage.MongoDB;
using ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

var apiVersioning = builder.Services.AddApiVersioning();

builder.AddServiceDefaults();
builder.AddDefaultOpenApi(apiVersioning);
builder.AddApplicationServices();
builder.Services.AddSailCore()
    .AddDatabaseStore();
builder.Services.AddGrpc();

var app = builder.Build();

var endpoint = app.NewVersionedApi();

endpoint.MapRouteApiV1();
endpoint.MapClusterApiV1();
endpoint.MapCertificateApiV1();

app.UseDefaultOpenApi();

app.MapGrpcService<RouteGrpcService>();
app.MapGrpcService<ClusterGrpcService>();
app.MapGrpcService<DestinationGrpcService>();
app.MapGrpcService<CertificateGrpcService>();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SailContext>();
    await context.InitializeAsync();
}

await app.RunAsync();
