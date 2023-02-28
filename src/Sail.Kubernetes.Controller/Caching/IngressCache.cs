using System.Collections.Immutable;
using k8s;
using k8s.Models;
using Microsoft.Extensions.Options;
using Microsoft.Kubernetes;
using Sail.Kubernetes.Controller.Models;
using Sail.Kubernetes.Controller.Services;

namespace Sail.Kubernetes.Controller.Caching;

public class IngressCache : ICache
{

    private readonly object _sync = new();
    private readonly Dictionary<string, IngressClassData> _ingressClassData = new();
    private readonly Dictionary<string, NamespaceCache> _namespaceCaches = new();
    private readonly SailOptions _options;
    private readonly ILogger<IngressCache> _logger;

    private bool _isDefaultController;

    public IngressCache(IOptions<SailOptions> options, ILogger<IngressCache> logger)
    {
        _options = options.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public bool IsSailIngress(IngressData ingress)
    { 
        return ingress.Spec.IngressClassName is not null
            ? _ingressClassData.ContainsKey(ingress.Spec.IngressClassName)
            : _isDefaultController;
    }

    public void Update(WatchEventType eventType, V1beta1Middleware middleware)
    {
        Namespace(middleware.Namespace()).Update(eventType, middleware);
    }

    public void Update(WatchEventType eventType, V1IngressClass ingressClass)
    {
        if (ingressClass is null)
        {
            throw new ArgumentNullException(nameof(ingressClass));
        }

        if (!string.Equals(_options.ControllerClass,ingressClass.Spec.Controller,StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation(
                "Ignoring {IngressClassNamespace}/{IngressClassName} as the spec.controller is not the same as this ingress",
                ingressClass.Metadata.NamespaceProperty,
                ingressClass.Metadata.Name);
            return;
        }

        var ingressClassName = ingressClass.Name();

        lock (_sync)
        {
            switch (eventType)
            {
                case WatchEventType.Added or WatchEventType.Modified:
                    _ingressClassData[ingressClassName] = new IngressClassData(ingressClass);
                    break;
                case WatchEventType.Deleted:
                    _ingressClassData.Remove(ingressClassName);
                    break;
            }

            _isDefaultController = _ingressClassData.Values.Any(ic => ic.IsDefault);
        }
    }

    public bool Update(WatchEventType eventType, V1Ingress ingress)
    {
        if (ingress is null)
        {
            throw new ArgumentNullException(nameof(ingress));
        }

        Namespace(ingress.Namespace()).Update(eventType, ingress);
        return true;
    }

    public ImmutableList<string> Update(WatchEventType eventType, V1Service service)
    {
        if (service is null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        return Namespace(service.Namespace()).Update(eventType, service);
    }

    public ImmutableList<string> Update(WatchEventType eventType, V1Endpoints endpoints)
    {
        return Namespace(endpoints.Namespace()).Update(eventType, endpoints);
    }

    public bool TryGetReconcileData(NamespacedName key, out ReconcileData data)
    {
        return Namespace(key.Namespace).TryLookup(key, out data);
    }

    public void GetKeys(List<NamespacedName> keys)
    {
        lock (_sync)
        {
            foreach (var (ns, cache) in _namespaceCaches)
            {
                cache.GetKeys(ns,keys);
            }
        }
    }

    public IEnumerable<IngressData> GetIngresses()
    {
        var ingresses = new List<IngressData>();
        lock (_sync)
        {
            foreach (var ns in _namespaceCaches)
            {
                ingresses.AddRange(ns.Value.GetIngresses());
            }
        }

        return ingresses;
    }

    public IEnumerable<MiddlewareData> GetMiddlewares()
    {
        var middlewares = new List<MiddlewareData>();
        lock (_sync)
        {
            foreach (var ns in _namespaceCaches)
            {
                middlewares.AddRange(ns.Value.GetMiddlewares());
            }
        }

        return middlewares;
    }

 

    private NamespaceCache Namespace(string key)
    {
        lock (_sync)
        {
            if (!_namespaceCaches.TryGetValue(key, out var value))
            {
                value = new NamespaceCache();
                _namespaceCaches.Add(key, value);
            }

            return value;
        }
    }
}