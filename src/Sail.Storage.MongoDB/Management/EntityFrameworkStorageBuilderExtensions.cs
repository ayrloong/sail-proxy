
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using Sail.Core;
using Sail.Core.Entities;
using Sail.Core.Stores;
using Sail.Storage.MongoDB.Stores;


namespace Sail.Storage.MongoDB.Management;

public static class EntityFrameworkStorageBuilderExtensions
{
    public static SailApplication AddDatabaseStore(this SailApplication application)
    {
        var services = application.Services;

        services.TryAddTransient<IRouteStore, RouteStore>();
        services.TryAddTransient<IClusterStore, ClusterStore>();
        services.TryAddTransient<ICertificateStore, CertificateStore>();
        services.TryAddScoped<SailContext>();
        
        return application;
    }
}


