using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Sail.Core.Entities;
using Sail.EntityFramework.Storage.Converters;

namespace Sail.EntityFramework.Storage;

public class ConfigurationContext : DbContext
{
    public ConfigurationContext(DbContextOptions<ConfigurationContext> options)
        : base(options)
    {
    }

    public DbSet<Cluster> Clusters { get; set; }
    public DbSet<Route> Routes { get; set; }
    public DbSet<Certificate> Certificates  { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ConfigureRouteContext();
        builder.ConfigureClusterContext();
        builder.ConfigureCertificateContext();
        
        base.OnModelCreating(builder);
    }
}

public class ConfigurationContextDesignFactory : IDesignTimeDbContextFactory<ConfigurationContext>
{
    public ConfigurationContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ConfigurationContext>();
        const string connectionStrings =
            "Server=localhost;Port=5432;Database=sail;User Id=postgres;Password=postgres";

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
            route.Property(x => x.AuthorizationPolicy).HasMaxLength(100).IsRequired(false);
            route.Property(x => x.RateLimiterPolicy).HasMaxLength(100).IsRequired(false);
            route.Property(x => x.CorsPolicy).HasMaxLength(100).IsRequired(false);
            route.Property(x => x.TimeoutPolicy).HasMaxLength(100).IsRequired(false);
            route.Property(x => x.Timeout).IsRequired(false);
            route.Property(x => x.MaxRequestBodySize).IsRequired(false);
            route.Property(x => x.CreatedAt).HasDefaultValue(DateTimeOffset.Now);
            route.Property(x => x.UpdatedAt).HasDefaultValue(DateTimeOffset.Now);

            route.HasOne(x => x.Match)
                .WithMany()
                .HasForeignKey(t => t.MatchId);
        });

        modelBuilder.Entity<RouteMatch>(match =>
        {
            match.HasKey(x => x.Id);

            match.Property(x => x.Path).HasMaxLength(200);
            match.Property(x => x.Hosts).HasConversion(StringArrayToJsonConverter.Instance).IsRequired(false);
            match.Property(x => x.Methods).HasConversion(StringArrayToJsonConverter.Instance).IsRequired(false);

            match.HasMany(x => x.Headers).WithOne(x => x.Match).HasForeignKey(x => x.MatchId);
            match.HasMany(x => x.QueryParameters).WithOne(x => x.Match).HasForeignKey(x => x.MatchId);
        });

        modelBuilder.Entity<RouteHeader>(header =>
        {
            header.HasKey(x => x.Id);

            header.Property(x => x.Name).HasMaxLength(200);
            header.Property(x => x.Mode).HasMaxLength(20);
            header.Property(x => x.IsCaseSensitive).HasMaxLength(10);
            header.Property(x => x.Values).HasConversion(StringArrayToJsonConverter.Instance).IsRequired(false);
        });

        modelBuilder.Entity<RouteQueryParameter>(query =>
        {
            query.HasKey(x => x.Id);

            query.Property(x => x.Name).HasMaxLength(200);
            query.Property(x => x.Mode).HasMaxLength(20);
            query.Property(x => x.IsCaseSensitive).HasMaxLength(10);
            query.Property(x => x.Values).HasConversion(StringArrayToJsonConverter.Instance).IsRequired(false);
        });
    }

    public static void ConfigureClusterContext(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cluster>(cluster =>
        {
            cluster.HasKey(x => x.Id);

            cluster.Property(x => x.Name).HasMaxLength(200);
            cluster.Property(x => x.LoadBalancingPolicy).HasMaxLength(100).IsRequired(false);
            cluster.Property(x => x.CreatedAt).HasDefaultValue(DateTimeOffset.Now);
            cluster.Property(x => x.UpdatedAt).HasDefaultValue(DateTimeOffset.Now);

            cluster.HasMany(x => x.Destinations).WithOne(x => x.Cluster).HasForeignKey(x => x.ClusterId);
        });

        modelBuilder.Entity<Destination>(destination =>
        {
            destination.HasKey(x => x.Id);

            destination.Property(x => x.Address).HasMaxLength(200);
            destination.Property(x => x.Host).HasMaxLength(200).IsRequired(false);
            destination.Property(x => x.Health).HasMaxLength(200).IsRequired(false);
        });
    }

    public static void ConfigureCertificateContext(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Certificate>(certificate =>
        {
            certificate.HasKey(x => x.Id);

            certificate.Property(x => x.Cert).HasMaxLength(4000);
            certificate.Property(x => x.Key).HasMaxLength(4000);
            certificate.Property(x => x.CreatedAt).HasDefaultValue(DateTimeOffset.Now);
            certificate.Property(x => x.UpdatedAt).HasDefaultValue(DateTimeOffset.Now);

        });
        modelBuilder.Entity<Sni>(sni =>
        {
            sni.HasKey(x => x.Id);

            sni.Property(x => x.Name).HasMaxLength(200);
            sni.Property(x => x.HostName).HasMaxLength(200);
            sni.Property(x => x.CreatedAt).HasDefaultValue(DateTimeOffset.Now);
            sni.Property(x => x.UpdatedAt).HasDefaultValue(DateTimeOffset.Now);

            sni.HasOne(x => x.Certificate)
                .WithMany()
                .HasForeignKey(t => t.CertificateId);
        });
    }
}