using k8s;
using k8s.Models;
using Microsoft.Kubernetes;
using System.Collections.Immutable;
using Sail.Kubernetes.Controller.Models;
using Sail.Kubernetes.Controller.Services;

namespace Sail.Kubernetes.Controller.Caching;

public class NamespaceCache
{
    private readonly object _sync = new();

    private readonly Dictionary<string, ImmutableList<string>> _ingressToServiceNames = new();
    private readonly Dictionary<string, ImmutableList<string>> _serviceToIngressNames = new();

    private readonly Dictionary<string, GatewayData> _gatewayData = new();
    private readonly Dictionary<string, IngressData> _ingressData = new();
    private readonly Dictionary<string, HttpRouteData> _httpRouteData = new();
    private readonly Dictionary<string, ServiceData> _serviceData = new();
    private readonly Dictionary<string, Endpoints> _endpointsData = new();

    public void Update(WatchEventType eventType, V1Ingress ingress)
    {
        if (ingress is null)
        {
            throw new ArgumentNullException(nameof(ingress));
        }
        var serviceNames = ImmutableList<string>.Empty;
        if (eventType is WatchEventType.Added or WatchEventType.Modified)
        {
            // If the ingress exists, list out the related services
            var spec = ingress.Spec;
            var defaultBackend = spec?.DefaultBackend;
            var defaultService = defaultBackend?.Service;
            if (!string.IsNullOrEmpty(defaultService?.Name))
            {
                serviceNames = serviceNames.Add(defaultService.Name);
            }

            foreach (var rule in spec.Rules ?? Enumerable.Empty<V1IngressRule>())
            {
                var http = rule.Http;
                foreach (var path in http.Paths ?? Enumerable.Empty<V1HTTPIngressPath>())
                {
                    var backend = path.Backend;
                    var service = backend.Service;

                    if (!serviceNames.Contains(service.Name))
                    {
                        serviceNames = serviceNames.Add(service.Name);
                    }
                }
            }
        }

        var ingressName = ingress.Name();
        lock (_sync)
        {
            var serviceNamesPrevious = ImmutableList<string>.Empty;
            switch (eventType)
            {
                case WatchEventType.Added or WatchEventType.Modified:
                {
                    // If the ingress exists then remember details
                    _ingressData[ingressName] = new IngressData(ingress);

                    if (_ingressToServiceNames.TryGetValue(ingressName, out serviceNamesPrevious))
                    {
                        _ingressToServiceNames[ingressName] = serviceNames;
                    }
                    else
                    {
                        serviceNamesPrevious = ImmutableList<string>.Empty;
                        _ingressToServiceNames.Add(ingressName, serviceNames);
                    }

                    break;
                }
                case WatchEventType.Deleted:
                {
                    // otherwise clear out details
                    _ingressData.Remove(ingressName);

                    if (_ingressToServiceNames.TryGetValue(ingressName, out serviceNamesPrevious))
                    {
                        _ingressToServiceNames.Remove(ingressName);
                    }

                    break;
                }
            }

            // update cross-reference for new ingress-to-services linkage not previously known
            foreach (var serviceName in serviceNames.Where(serviceName => !serviceNamesPrevious.Contains(serviceName)))
            {
                if (_serviceToIngressNames.TryGetValue(serviceName, out var ingressNamesPrevious))
                {
                    _serviceToIngressNames[serviceName] = _serviceToIngressNames[serviceName].Add(ingressName);
                }
                else
                {
                    _serviceToIngressNames.Add(serviceName, ImmutableList<string>.Empty.Add(ingressName));
                }
            }

            // remove cross-reference for previous ingress-to-services linkage no longer present
            foreach (var serviceName in serviceNamesPrevious.Where(serviceName => !serviceNames.Contains(serviceName)))
            {
                _serviceToIngressNames[serviceName] = _serviceToIngressNames[serviceName].Remove(ingressName);
            }
        }
    }

    public void Update(WatchEventType eventType, V1beta1Gateway gateway)
    {
        if (gateway is null)
        {
            throw new ArgumentNullException(nameof(gateway));
        }
        
    }

    public ImmutableList<string> Update(WatchEventType eventType, V1beta1HttpRoute httpRoute)
    {
        if (httpRoute is null)
        {
            throw new ArgumentNullException(nameof(httpRoute));
        }

        var httpRouteName = httpRoute.Name();
        lock (_sync)
        {
            switch (eventType)
            {
                case WatchEventType.Added or WatchEventType.Modified:
                    _httpRouteData[httpRouteName] = new HttpRouteData(httpRoute);
                    break;
                case WatchEventType.Deleted:
                    _httpRouteData.Remove(httpRouteName);
                    break;
            }
        }

        return ImmutableList<string>.Empty;
    }

    public ImmutableList<string> Update(WatchEventType eventType, V1Service service)
    {
        if (service is null)
        {
            throw new ArgumentNullException(nameof(service));
        }

        var serviceName = service.Name();
        lock (_sync)
        {
            if (eventType is WatchEventType.Added or WatchEventType.Modified)
            {
                _serviceData[serviceName] = new ServiceData(service);
            }
            else if (eventType == WatchEventType.Deleted)
            {
                _serviceData.Remove(serviceName);
            }

            if (_serviceToIngressNames.TryGetValue(serviceName, out var ingressNames))
            {
                return ingressNames;
            }
            else
            {
                return ImmutableList<string>.Empty;
            }
        }
    }

    public void GetKeys(string ns, List<NamespacedName> keys)
    {
        if (keys is null)
        {
            throw new ArgumentNullException(nameof(keys));
        }

        lock (_sync)
        {
            foreach (var name in _ingressData.Keys)
            {
                keys.Add(new NamespacedName(ns, name));
            }
        }
    }

    public ImmutableList<string> Update(WatchEventType eventType, V1Endpoints endpoints)
    {
        if (endpoints is null)
        {
            throw new ArgumentNullException(nameof(endpoints));
        }

        var serviceName = endpoints.Name();
        lock (_sync)
        {
            switch (eventType)
            {
                case WatchEventType.Added:
                case WatchEventType.Modified:
                    _endpointsData[serviceName] = new Endpoints(endpoints);
                    break;
                case WatchEventType.Deleted:
                    _endpointsData.Remove(serviceName);
                    break;
            }

            return _serviceToIngressNames.TryGetValue(serviceName, out var ingressNames)
                ? ingressNames
                : ImmutableList<string>.Empty;
        }
    }

    public IEnumerable<IngressData> GetIngresses()
    {
        return _ingressData.Values;
    }

    public IEnumerable<GatewayData> GetGateways()
    {
        return _gatewayData.Values;
    }

    public bool IngressExists(V1Ingress ingress)
    {
        return _ingressData.ContainsKey(ingress.Name());
    }
    
    public bool GatewayExists(V1beta1Gateway gateway)
    {
        return _gatewayData.ContainsKey(gateway.Name());
    }
    public bool TryLookup(NamespacedName key, out ReconcileData data)
    {
        var endspointsList = new List<Endpoints>();
        var servicesList = new List<ServiceData>();

        lock (_sync)
        {
            if (!_ingressData.TryGetValue(key.Name, out var ingress))
            {
                data = default;
                return false;
            }

            if (_ingressToServiceNames.TryGetValue(key.Name, out var serviceNames))
            {
                foreach (var serviceName in serviceNames)
                {
                    if (_serviceData.TryGetValue(serviceName, out var serviceData))
                    {
                        servicesList.Add(serviceData);
                    }

                    if (_endpointsData.TryGetValue(serviceName, out var endpoints))
                    {
                        endspointsList.Add(endpoints);
                    }
                }
            }

            if (_serviceData.Count == 0)
            {
                data = default;
                return false;
            }

            data = new ReconcileData(ingress, servicesList, endspointsList);
            return true;
        }
    }
}