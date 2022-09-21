using k8s;
using k8s.Models;
using Microsoft.Kubernetes;
using System.Collections.Immutable;
using Sail.Kubernetes.Controller.Models;
using Sail.Kubernetes.Controller.Services;

namespace Sail.Kubernetes.Controller.Caching;

public interface ICache
{

    bool Update(WatchEventType eventType, V1beta1GatewayClass gatewayClass);
    void Update(WatchEventType eventType, V1beta1Gateway gateway);
    void Update(WatchEventType eventType, V1beta1HttpRoute httpRoute);
    void Update(WatchEventType eventType, V1IngressClass ingressClass);
    bool Update(WatchEventType eventType, V1Ingress ingress);
    ImmutableList<string> Update(WatchEventType eventType, V1Service service);
    ImmutableList<string> Update(WatchEventType eventType, V1Endpoints endpoints);
    bool TryGetReconcileData(NamespacedName key, out ReconcileData data);
    void GetKeys(List<NamespacedName> keys);
    IEnumerable<IngressData> GetIngresses();
}
