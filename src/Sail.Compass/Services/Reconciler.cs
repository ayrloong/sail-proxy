using Sail.Api.V1;
using Sail.Compass.Caching;
using Sail.Compass.Converters;
using Sail.Core.ConfigProvider;

namespace Sail.Compass.Services;

public class Reconciler(ICache cache, IUpdateConfig updateConfig) : IReconciler
{
    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        var configContext = new YarpConfigContext();
        var clusters = cache.GetClusters();
        var routes = cache.GetRoutes();
        var context = new DataSourceContext(clusters, routes);

        Parser.ConvertFromDataSource(context, configContext);

        await updateConfig.UpdateAsync(configContext.Routes, configContext.Clusters,
            cancellationToken);
    }
}