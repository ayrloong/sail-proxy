using Microsoft.Extensions.Hosting;
using Sail.Core.Configuration.ConfigProvider;
using Sail.Core.Converters;
using Sail.Core.Stores;

namespace Sail.Core.Hosting;

public class ProxyHostedService(
    IUpdateConfig proxyConfigProvider,
    IRouteStore routeStore,
    IClusterStore clusterStore) : BackgroundService, IDisposable
{
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
             await UpdateAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }



    private async Task UpdateAsync(CancellationToken cancellationToken)
    {
        try
        {
            var configContext = new YarpConfigContext();
            var clusters = await clusterStore.GetAsync();
            var routes = await routeStore.GetAsync();
            var context = new DataSourceContext(clusters, routes);
            
            Parser.ConvertFromDataSource(context, configContext);
            
            await proxyConfigProvider.UpdateAsync(configContext.Routes, configContext.Clusters,
                cancellationToken);
            await Task.Yield();
        }
        catch (Exception e)
        {
            // ignored
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
       
    }

    public void Dispose()
    {
      
    }
}