using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Sail.Core.Configuration.ConfigProvider;
using Sail.Core.Converters;
using Sail.Core.Options;
using Sail.Core.Stores;

namespace Sail.Core.Hosting;

public class ProxyHostedService(
    IUpdateConfig proxyConfigProvider,
    IServiceScopeFactory serviceScopeFactory,
    IOptions<DefaultOptions> options) : BackgroundService
{

    private readonly DefaultOptions _options = options.Value ?? throw new ArgumentNullException(nameof(options));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(_options.InitializePeriodTime));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await UpdateAsync(stoppingToken);
        }
    }


    private async Task UpdateAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var clusterStore = scope.ServiceProvider.GetService<IClusterStore>() ?? throw new OperationException();
            var routeStore = scope.ServiceProvider.GetService<IRouteStore>() ?? throw new OperationException();
            
            var configContext = new YarpConfigContext();
            var clusters = await clusterStore.GetAsync();
            var routes = await routeStore.GetAsync();
            var context = new DataSourceContext(clusters, routes);

            Parser.ConvertFromDataSource(context, configContext);

            await proxyConfigProvider.UpdateAsync(configContext.Routes, configContext.Clusters,
                cancellationToken);
        }
        catch (Exception e)
        {
            // ignored
        }
    }
}