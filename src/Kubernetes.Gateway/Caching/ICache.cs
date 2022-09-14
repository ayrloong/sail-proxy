using System.Collections.Immutable;
using k8s;
using k8s.Models;
using Microsoft.Kubernetes;
using Sail.Kubernetes.Gateway.Models;
using Sail.Kubernetes.Gateway.Services;

namespace Sail.Kubernetes.Gateway.Caching;

public interface ICache
{
    bool Update(WatchEventType eventType, V1beta1HttpRoute httpRoute);
    void Update(WatchEventType eventType, V1beta1GatewayClass gatewayClass);
    bool Update(WatchEventType eventType, V1beta1Gateway gateway);
    ImmutableList<string> Update(WatchEventType eventType, V1Service service);
    ImmutableList<string> Update(WatchEventType eventType, V1Endpoints endpoints);
    bool TryGetReconcileData(NamespacedName key, out ReconcileData data);
    void GetKeys(List<NamespacedName> keys);
    IEnumerable<GatewayData> GetGatewayes();
}