using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Sail.Core.Entities;

namespace Sail.EntityFramework.Storage;

public class ConfigurationContext : DbContext
{
    public ConfigurationContext(DbContextOptions<ConfigurationContext> options)
        : base(options)
    {
    }

    public DbSet<Cluster> Clusters { get; set; }
    public DbSet<Route> Routes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ConfigureRouteContext();
        builder.ConfigureClusterContext();
 
        base.OnModelCreating(builder);
    }
}

public class ConfigurationContextDesignFactory : IDesignTimeDbContextFactory<ConfigurationContext>
{
    public ConfigurationContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ConfigurationContext>();
        const string connectionStrings =
            "server=localhost;port=5432;database=sail;user=root;password=123456";

        optionsBuilder.UseNpgsql(connectionStrings);
        return new ConfigurationContext(optionsBuilder.Options);
    }
}

public static class ModelBuilderExtensions
{
    public static void ConfigureRouteContext(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Route>(route =>
        {
            route.HasKey(x => x.Id);

            route.Property(x => x.Name).HasMaxLength(200);
            
        });
        
        modelBuilder.Entity<RouteMatch>(match => { match.HasKey(x => x.Id); });
        modelBuilder.Entity<RouteHeader>(header => { header.HasKey(x => x.Id); });
        modelBuilder.Entity<RouteQueryParameter>(query => { query.HasKey(x => x.Id); });

    }

    public static void ConfigureClusterContext(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cluster>(cluster => { cluster.HasKey(x => x.Id); });
    }
}