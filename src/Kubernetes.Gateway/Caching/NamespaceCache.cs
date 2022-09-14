using System.Collections.Immutable;
using k8s;
using k8s.Models;
using Microsoft.Kubernetes;
using Sail.Kubernetes.Gateway.Models;
using Sail.Kubernetes.Gateway.Services;

namespace Sail.Kubernetes.Gateway.Caching;

public class NamespaceCache
{
    private readonly object _sync = new object();

    private readonly Dictionary<string, ImmutableList<string>> _gatewayToServiceNames = new();
    private readonly Dictionary<string, ImmutableList<string>> _serviceToGatewayNames = new();
    private readonly Dictionary<string, GatewayData> _gatewayData = new();
    private readonly Dictionary<string, ServiceData> _serviceData = new();
    private readonly Dictionary<string, Endpoints> _endpointsData = new();

    public void Update(WatchEventType eventType, V1beta1Gateway gateway)
    {
        if (gateway is null)
        {
            throw new ArgumentNullException(nameof(gateway));
        }

        var serviceNames = ImmutableList<string>.Empty;
        if (eventType is WatchEventType.Added or WatchEventType.Modified)
        {
            var spec = gateway.Spec;
        }

        var gatewayName = gateway.Name();
        lock (_sync)
        {
            var serviceNamesPrevious = ImmutableList<string>.Empty;
            switch (eventType)
            {
                case WatchEventType.Added or WatchEventType.Modified:
                {
                    _gatewayData[gatewayName] = new GatewayData(gateway);
                    if (_gatewayToServiceNames.TryGetValue(gatewayName, out serviceNamesPrevious))
                    {
                        _gatewayToServiceNames[gatewayName] = serviceNames;
                    }
                    else
                    {
                        serviceNamesPrevious = ImmutableList<string>.Empty;
                        _gatewayToServiceNames.Add(gatewayName, serviceNames);
                    }
                    break;
                }
                case WatchEventType.Deleted:
                    _gatewayData.Remove(gatewayName);
                    if (_gatewayToServiceNames.TryGetValue(gatewayName, out serviceNamesPrevious))
                    {
                        _gatewayToServiceNames.Remove(gatewayName);
                    }

                    break;
            }

            foreach (var serviceName in serviceNames)
            {
                if (!serviceNamesPrevious.Contains(serviceName))
                {
                    if (_serviceToGatewayNames.TryGetValue(serviceName, out var gatewayNamesPrevious))
                    {
                        _serviceToGatewayNames[serviceName] = _serviceToGatewayNames[serviceName].Add(gatewayName);
                    }
                    else
                    {
                        _serviceToGatewayNames.Add(serviceName, ImmutableList<string>.Empty.Add(gatewayName));
                    }
                }
            }

            foreach (var serviceName in serviceNamesPrevious)
            {
                if (!serviceNames.Contains(serviceName))
                {
                    _serviceToGatewayNames[serviceName] = _serviceToGatewayNames[serviceName].Remove(gatewayName);
                }
            }
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
                case WatchEventType.Added:
                case WatchEventType.Modified:
                    _serviceData[serviceName] = new ServiceData(service);
                    break;
                case WatchEventType.Deleted:
                    _serviceData.Remove(serviceName);
                    break;
            }

            return _serviceToGatewayNames.TryGetValue(serviceName, out var gatewayNames)
                ? gatewayNames
                : ImmutableList<string>.Empty;
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

            return _serviceToGatewayNames.TryGetValue(serviceName, out var gatewayNames)
                ? gatewayNames
                : ImmutableList<string>.Empty;
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
            keys.AddRange(_gatewayData.Keys.Select(name => new NamespacedName(ns, name)));
        }
    }

    public IEnumerable<GatewayData> GetGatewayes()
    {
        return _gatewayData.Values;
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
            if (!_gatewayData.TryGetValue(key.Name, out var gateway))
            {
                data = default;
                return false;
            }

            if (_gatewayToServiceNames.TryGetValue(key.Name, out var serviceNames))
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

            if (!_serviceData.Any())
            {
                data = default;
                return false;
            }

            data = new ReconcileData(gateway, servicesList, endspointsList);
            return true;
        }
    }
}