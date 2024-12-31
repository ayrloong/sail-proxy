using Sail.Apis;
using Sail.Core.Management;
using Sail.EntityFramework.Storage.Management;
using Sail.Extensions;
using Sail.Grpc;
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

app.Run();

