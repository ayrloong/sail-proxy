using System.Collections.Immutable;
using k8s;
using k8s.Models;
using Microsoft.Extensions.Options;
using Microsoft.Kubernetes;
using Newtonsoft.Json;
using Sail.Kubernetes.Controller.Models;
using Sail.Kubernetes.Controller.Services;

namespace Sail.Kubernetes.Controller.Caching;

public class ControllerCache : ICache
{
    private readonly object _sync = new();
    private readonly Dictionary<string, IngressClassData> _ingressClassData = new();
    private readonly Dictionary<string, GatewayClassData> _gatewayClassData = new();
    private readonly Dictionary<string, NamespaceCache> _namespaceCaches = new();
    private readonly SailOptions _options;
    private readonly ILogger<ControllerCache> _logger;

    private bool _isDefaultIngressController;
    private bool _isDefaultGatewayController;
    public ControllerCache(IOptions<SailOptions> options, ILogger<ControllerCache> logger)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void Update(WatchEventType eventType, V1beta1GatewayClass gatewayClass)
    {
        if (gatewayClass is null)
        {
            throw new ArgumentNullException(nameof(gatewayClass));
        }

        if (!string.Equals(_options.GatewayControllerClass, gatewayClass.Spec.ControllerName,
                StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation(
                "Ignoring {GatewayClassNamespace}/{GatewayClassName} as the spec.controller is not the same as this ingress",
                gatewayClass.Metadata.NamespaceProperty,
                gatewayClass.Metadata.Name);
            return;
        }

        var gatewayClassName = gatewayClass.Name();
        lock (_sync)
        {
            switch (eventType)
            {
                case WatchEventType.Added or WatchEventType.Modified:
                    _gatewayClassData[gatewayClassName] = new GatewayClassData(gatewayClass);
                    break;
                case WatchEventType.Deleted:
                    _gatewayClassData.Remove(gatewayClassName);
                    break;
            }

            _isDefaultGatewayController = _gatewayClassData.Values.Any(ic => ic.IsDefault);
        }
    }

    public bool Update(WatchEventType eventType, V1beta1Gateway gateway)
    {
        if (gateway is null)
        {
            throw new ArgumentNullException(nameof(gateway));
        }

        if (IsGatewayClass(gateway.Spec))
        {
            Namespace(gateway.Namespace()).Update(eventType, gateway);
            return true;
        }

        if (eventType == WatchEventType.Modified && Namespace(gateway.Namespace()).GatewayExists(gateway))
        {
            _logger.LogInformation("Removing gateway {GatewayNamespace}/{GatewayName} because of unknown gateway class",
                gateway.Metadata.NamespaceProperty, gateway.Metadata.Name);
            Namespace(gateway.Namespace()).Update(WatchEventType.Deleted, gateway);
            return true;
        }
        
        _logger.LogInformation("Ignoring gateway {GatewayNamespace}/{GatewayName} because of gateway class",
            gateway.Metadata.NamespaceProperty, gateway.Metadata.Name);
        return false;
    }

    public ImmutableList<string> Update(WatchEventType eventType, V1beta1HttpRoute httpRoute)
    {
        if (httpRoute is null)
        {
            throw new ArgumentNullException(nameof(httpRoute));
        }

        return Namespace(httpRoute.Namespace()).Update(eventType, httpRoute);
    }

    public void Update(WatchEventType eventType, V1IngressClass ingressClass)
    {
        if (ingressClass is null)
        {
            throw new ArgumentNullException(nameof(ingressClass));
        }

        if (!string.Equals(_options.IngressControllerClass, ingressClass.Spec.Controller, StringComparison.OrdinalIgnoreCase))
        {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            _logger.LogInformation(
                "Ignoring {IngressClassNamespace}/{IngressClassName} as the spec.controller is not the same as this ingress",
                ingressClass.Metadata.NamespaceProperty,
                ingressClass.Metadata.Name);
#pragma warning restore CA1303 // Do not pass literals as localized parameters
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

            _isDefaultIngressController = _ingressClassData.Values.Any(ic => ic.IsDefault);
        }
    }

    public bool Update(WatchEventType eventType, V1Ingress ingress)
    {
        if (ingress is null)
        {
            throw new ArgumentNullException(nameof(ingress));
        }
        
        if (IsIngressClass(ingress.Spec))
        {
            Namespace(ingress.Namespace()).Update(eventType, ingress);
            return true;
        }

#pragma warning disable CA1303 // Do not pass literals as localized parameters
        if (eventType == WatchEventType.Modified && Namespace(ingress.Namespace()).IngressExists(ingress))
        {
            // Special handling for an ingress that has the ingressClassName removed
            _logger.LogInformation("Removing ingress {IngressNamespace}/{IngressName} because of unknown ingress class",
                ingress.Metadata.NamespaceProperty, ingress.Metadata.Name);
            Namespace(ingress.Namespace()).Update(WatchEventType.Deleted, ingress);
            return true;
        }

        _logger.LogInformation("Ignoring ingress {IngressNamespace}/{IngressName} because of ingress class",
            ingress.Metadata.NamespaceProperty, ingress.Metadata.Name);
#pragma warning restore CA1303 // Do not pass literals as localized parameters

        return false;
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
                cache.GetKeys(ns, keys);
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

    public IEnumerable<GatewayData> GetGateways()
    {
        var gateways = new List<GatewayData>();

        lock (_sync)
        {
            foreach (var ns in _namespaceCaches)
            {
                gateways.AddRange(ns.Value.GetGateways());
            }
        }

        return gateways;
    }

    private bool IsIngressClass(V1IngressSpec spec)
    {
        if (spec.IngressClassName is not null)
        {
            lock (_sync)
            {
                return _ingressClassData.ContainsKey(spec.IngressClassName);
            }
        }

        return _isDefaultIngressController;
    }

    private bool IsGatewayClass(V1beta1GatewaySpec spec)
    {
        if (spec.GatewayClassName is not null)
        {
            lock (_sync)
            {
                return _gatewayClassData.ContainsKey(spec.GatewayClassName);
            }
        }

        return _isDefaultGatewayController;
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