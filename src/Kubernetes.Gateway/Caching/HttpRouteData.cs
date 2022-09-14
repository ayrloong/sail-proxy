using k8s.Models;
using Sail.Kubernetes.Gateway.Models;

namespace Sail.Kubernetes.Gateway.Caching;
public struct HttpRouteData
{
    public HttpRouteData(V1beta1HttpRoute httpRoute)
    {
        if (httpRoute is null)
        {
            throw new ArgumentNullException(nameof(httpRoute));
        }

        Spec = httpRoute.Spec;
        Metadata = httpRoute.Metadata;
    }

    public V1beta1HttpRouteSpec Spec { get; set; }
    public V1ObjectMeta Metadata { get; set; }
}