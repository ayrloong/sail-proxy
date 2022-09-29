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
    private readonly Dictionary<string, ImmutableList<string>> _gatewayToHttpRouteNames = new();
    private readonly Dictionary<string, ImmutableList<string>> _httpRouteToServiceNames = new();
    private readonly Dictionary<string, ImmutableList<string>> _serviceToHttpRouteNames = new();

    private readonly Dictionary<string, GatewayData> _gatewayData = new();
    private readonly Dictionary<string, HttpRouteData> _httpRouteData = new();
    private readonly Dictionary<string, ServiceData> _serviceData = new();
    private readonly Dictionary<string, Endpoints> _endpointsData = new();

    public void Update(WatchEventType eventType, V1beta1Gateway gateway)
    {
        if (gateway is null)
        {
            throw new ArgumentNullException(nameof(gateway));
        }

        var gatewayName = gateway.Name();

        lock (_sync)
        {
            switch (eventType)
            {
                case WatchEventType.Added or WatchEventType.Modified:
                {
                    _gatewayData[gatewayName] = new GatewayData(gateway);
                    break;
                }
                case WatchEventType.Deleted:
                {
                    _gatewayData.Remove(gatewayName);
                    break;
                }
            }
        }
    }

    public ImmutableList<string> Update(WatchEventType eventType, V1beta1HttpRoute httpRoute)
    {
        if (httpRoute is null)
        {
            throw new ArgumentNullException(nameof(httpRoute));
        }

        var parentRef = httpRoute.Spec.ParentRefs.FirstOrDefault();
        var gatewayName = parentRef.Name;
        var httpRouteName = httpRoute.Name();
        var serviceNames = ImmutableList<string>.Empty;
        
        if (eventType is WatchEventType.Added or WatchEventType.Modified)
        {
            var spec = httpRoute.Spec;
            foreach (var service in spec.Rules.SelectMany(rule =>
                         rule.BackendRefs.Where(service => !serviceNames.Contains(service.Name))))
            {
                serviceNames = serviceNames.Add(service.Name);
            }

            if (_gatewayToHttpRouteNames.TryGetValue(gatewayName, out var httpRouteNamesPrevious))
            {
                _gatewayToHttpRouteNames[gatewayName] = httpRouteNamesPrevious.Add(httpRouteName);
            }
            else
            {
                _gatewayToHttpRouteNames.Add(gatewayName, ImmutableList<string>.Empty.Add(httpRouteName));
            }
        }

        if (eventType is WatchEventType.Deleted)
        {
            if (_gatewayToHttpRouteNames.TryGetValue(gatewayName, out var httpRouteNamesPrevious))
            {
                _gatewayToHttpRouteNames[gatewayName] = httpRouteNamesPrevious.Remove(httpRouteName);
            }
        }

        lock (_sync)
        {
            var serviceNamesPrevious = ImmutableList<string>.Empty;
            switch (eventType)
            {
                case WatchEventType.Added or WatchEventType.Modified:
                    _httpRouteData[httpRouteName] = new HttpRouteData(httpRoute);
                    if (_httpRouteToServiceNames.TryGetValue(httpRouteName, out serviceNamesPrevious))
                    {
                        _httpRouteToServiceNames[httpRouteName] = serviceNames;
                    }
                    else
                    {
                        serviceNamesPrevious = ImmutableList<string>.Empty;
                        _httpRouteToServiceNames.Add(httpRouteName, serviceNames);
                    }
                    break;
                case WatchEventType.Deleted:
                    _httpRouteData.Remove(httpRouteName);
                    if (_httpRouteToServiceNames.TryGetValue(httpRouteName, out serviceNamesPrevious))
                    {
                        _httpRouteToServiceNames.Remove(httpRouteName);
                    }
                    break;
            }

            foreach (var serviceName in serviceNames.Where(serviceName => !serviceNamesPrevious.Contains(serviceName)))
            {
                if (_serviceToHttpRouteNames.TryGetValue(serviceName, out var _))
                {
                    _serviceToHttpRouteNames[serviceName] = _serviceToHttpRouteNames[serviceName].Add(httpRouteName);
                }
                else
                {
                    _serviceToHttpRouteNames.Add(serviceName, ImmutableList<string>.Empty.Add(httpRouteName));
                }
            }

            // remove cross-reference for previous ingress-to-services linkage no longer present
            foreach (var serviceName in serviceNamesPrevious.Where(serviceName => !serviceNames.Contains(serviceName)))
            {
                _serviceToHttpRouteNames[serviceName] = _serviceToHttpRouteNames[serviceName].Remove(httpRouteName);
            }
            
            return _gatewayToHttpRouteNames.TryGetValue(gatewayName, out var httpRouteNames)
                ? httpRouteNames
                : ImmutableList<string>.Empty;
        }
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
            switch (eventType)
            {
                case WatchEventType.Added or WatchEventType.Modified:
                    _serviceData[serviceName] = new ServiceData(service);
                    break;
                case WatchEventType.Deleted:
                    _serviceData.Remove(serviceName);
                    break;
            }
        }

        return _serviceToHttpRouteNames.TryGetValue(serviceName, out var httpRouteNames)
            ? httpRouteNames
            : ImmutableList<string>.Empty;
    }

    public void GetKeys(string ns, List<NamespacedName> keys)
    {
        if (keys is null)
        {
            throw new ArgumentNullException(nameof(keys));
        }

        lock (_sync)
        {
            keys.AddRange(_gatewayData.Keys.Select(name => new NamespacedName(ns, name)));
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

            return _serviceToHttpRouteNames.TryGetValue(serviceName, out var httpRouteNames)
                ? httpRouteNames
                : ImmutableList<string>.Empty;
        }
    }

    public IEnumerable<GatewayData> GetGateways()
    {
        return _gatewayData.Values;
    }

    public bool TryLookup(NamespacedName key, out ReconcileData data)
    {
        var endspointsList = new List<Endpoints>();
        var servicesList = new List<ServiceData>();
        var httpRouteList = new List<HttpRouteData>();
        lock (_sync)
        {
            if (!_gatewayData.TryGetValue(key.Name, out var gateway))
            {
                data = default;
                return false;
            }

            if (_gatewayToHttpRouteNames.TryGetValue(key.Name, out var httpRouteNames))
            {
                foreach (var httpRouteName in httpRouteNames)
                {
                    if (_httpRouteData.TryGetValue(httpRouteName, out var httpRouteData))
                    {
                        httpRouteList.Add(httpRouteData);
                    }
                }
            }

            foreach (var httpRouteName in _gatewayToHttpRouteNames.Where(x => x.Key == key.Name)
                         .SelectMany(x => x.Value))
            {
                if (_httpRouteToServiceNames.TryGetValue(httpRouteName, out var serviceNames))
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
            }

            if (!_serviceData.Any())
            {
                data = default;
                return false;
            }

            data = new ReconcileData(gateway, httpRouteList, servicesList, endspointsList);
            return true;
        }
    }
}