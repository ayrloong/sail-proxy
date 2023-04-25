// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using k8s;
using k8s.Autorest;
using k8s.Exceptions;
using Microsoft.Extensions.Options;
using Microsoft.Kubernetes;
using Microsoft.Kubernetes.Client;
using Microsoft.Kubernetes.ResourceKinds;
using Microsoft.Kubernetes.Resources;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Class KubernetesCoreExtensions.
    /// </summary>
    public static class KubernetesCoreExtensions
    {
        /// <summary>
        /// Adds the kubernetes.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddKubernetesCore(this IServiceCollection services)
        {
            if (services.All(serviceDescriptor => serviceDescriptor.ServiceType != typeof(IKubernetes)))
            {
                services = services.Configure<KubernetesClientOptions>(options =>
                {
                    options.Configuration ??= KubernetesClientConfiguration.BuildDefaultConfig();
                });

                services = services.AddSingleton<IKubernetes>(sp =>
                {
                    var options = sp.GetRequiredService<IOptions<KubernetesClientOptions>>().Value;

                    return new k8s.Kubernetes(options.Configuration);
                });
            }

            if (services.All(serviceDescriptor => serviceDescriptor.ServiceType != typeof(IResourceSerializers)))
            {
                services = services.AddTransient<IResourceSerializers, ResourceSerializers>();
            }

            if (services.All(serviceDescriptor => serviceDescriptor.ServiceType != typeof(IResourcePatcher)))
            {
                services = services.AddTransient<IResourcePatcher, ResourcePatcher>();
            }

            if (services.All(serviceDescriptor => serviceDescriptor.ServiceType != typeof(IResourceKindManager)))
            {
                services = services.AddSingleton<IResourceKindManager, ResourceKindManager>();
            }

            return services;
        }
    }
}

namespace k8s
{
    public static class KubernetesHelpersExtensions
    {
        public static IAnyResourceKind AnyResourceKind(this IKubernetes client)
        {
            return new AnyResourceKind(client);
        }
    }

    public static class WatcherExtensions
    {
        public static Watcher<T> WatchResource<T, L>(
            this Task<HttpOperationResponse<L>> responseTask,
            Action<WatchEventType, T> onEvent,
            Action<Exception> onError = null,
            Action onClosed = null)
        {
            return new Watcher<T>(MakeStreamReaderCreator<T, L>(responseTask), onEvent, onError, onClosed);
        }

        private static Func<Task<TextReader>> MakeStreamReaderCreator<T, L>(Task<HttpOperationResponse<L>> responseTask)
        {
            return async () =>
            {
                var response = await responseTask.ConfigureAwait(false);

                if (response.Response.Content is not LineSeparatedHttpContent content)
                {
                    throw new KubernetesClientException("not a watchable request or failed response");
                }

                return content.StreamReader;
            };
        }

        public static Watcher<T> WatchResource<T, L>(
            this HttpOperationResponse<L> response,
            Action<WatchEventType, T> onEvent,
            Action<Exception> onError = null,
            Action onClosed = null)
        {
            return WatchResource(Task.FromResult(response), onEvent, onError, onClosed);
        }
    }
}