using Sail.Kubernetes.Controller.Certificates;

namespace Microsoft.AspNetCore.Hosting;

public static class KubernetesWebHostBuilderExtensions
{
    public static IWebHostBuilder UseKubernetesCertificateSelector(this IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        builder.ConfigureKestrel(kestrelOptions =>
        {
            kestrelOptions.ConfigureHttpsDefaults(httpsOptions =>
            {
                var selector = kestrelOptions.ApplicationServices.GetService<IServerCertificateSelector>();
                if (selector is null)
                {
                    throw new InvalidOperationException(
                        "Missing required services. Did you call '.AddKubernetesReverseProxy()' when configuring services?");
                }

                httpsOptions.ServerCertificateSelector = (connectionContext, domainName) =>
                    selector.GetCertificate(connectionContext, domainName);
            });
        });

        return builder;
    }
}