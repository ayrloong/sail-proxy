using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sail.Core;
using Sail.Core.Options;
using Sail.Core.Stores;
using Sail.EntityFramework.Storage.Stores;


namespace Sail.EntityFramework.Storage.Management;

public static class EntityFrameworkStorageBuilderExtensions
{
    public static SailApplication AddDatabaseStore(this SailApplication application)
    {
        var services = application.Services;
        
        services.TryAddTransient<IRouteStore, RouteStore>();
        services.TryAddTransient<IClusterStore, ClusterStore>();
        services.TryAddTransient<ICertificateStore, CertificateStore>();
        
        services.AddDbContext<ConfigurationContext>((sp, options) =>
        {
            var database = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            options.UseNpgsql(database.ConnectionString);
        });
        return application;
    }
}


