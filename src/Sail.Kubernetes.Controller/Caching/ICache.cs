using System.Collections.Immutable;
using k8s;
using k8s.Models;
using Microsoft.Kubernetes;
using Sail.Kubernetes.Controller.Models;
using Sail.Kubernetes.Controller.Services;

namespace Sail.Kubernetes.Controller.Caching;

public interface ICache
{
    bool IsSailIngress(IngressData ingress);
    void Update(WatchEventType eventType,V1beta1Middleware middleware);
    void Update(WatchEventType eventType, V1IngressClass ingressClass);
    bool Update(WatchEventType eventType, V1Ingress ingress);
    ImmutableList<string> Update(WatchEventType eventType, V1Service service);
    ImmutableList<string> Update(WatchEventType eventType, V1Endpoints endpoints);
    bool TryGetReconcileData(NamespacedName key, out ReconcileData data);
    void GetKeys(List<NamespacedName> keys);
    IEnumerable<IngressData> GetIngresses();
    IEnumerable<MiddlewareData> GetMiddlewares();
}