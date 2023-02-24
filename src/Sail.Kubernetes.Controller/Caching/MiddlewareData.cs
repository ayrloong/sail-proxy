using k8s.Models;
using Sail.Kubernetes.Controller.Models;

namespace Sail.Kubernetes.Controller.Caching;

public struct MiddlewareData
{
    public MiddlewareData(V1beta1Middleware middleware)
    {
        if (middleware is null)
        {
            throw new ArgumentNullException(nameof(middleware));
        }

        Spec = middleware.Spec;
        Metadata = middleware.Metadata;
    }

    public V1beta1MiddlewareSpec Spec { get; set; }
    public V1ObjectMeta Metadata { get; set; }
}