using Microsoft.Extensions.DependencyInjection;
using Sail.Core.Certificates;

namespace Microsoft.AspNetCore.Hosting;

public static class SailWebHostBuilderExtensions
{
    public static IWebHostBuilder UseCertificateSelector(this IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        builder.ConfigureKestrel(kestrelOptions =>
        {
            kestrelOptions.ConfigureHttpsDefaults(httpsOptions =>
            {
                var selector = kestrelOptions.ApplicationServices.GetService<IServerCertificateSelector>();
                if (selector is null)
                {
                    throw new InvalidOperationException();
                }

                httpsOptions.ServerCertificateSelector = (connectionContext, domainName) =>
                    selector.GetCertificate(connectionContext, domainName);
            });
        });

        return builder;
    }
}