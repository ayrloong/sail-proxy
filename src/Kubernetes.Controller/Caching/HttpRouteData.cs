using k8s.Models;
using Sail.Kubernetes.Controller.Models;

namespace Sail.Kubernetes.Controller.Caching;

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