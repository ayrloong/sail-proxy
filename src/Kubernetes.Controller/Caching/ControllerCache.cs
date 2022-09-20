﻿using System.Collections.Immutable;
using k8s;
using k8s.Models;
using Microsoft.Extensions.Options;
using Microsoft.Kubernetes;
using Newtonsoft.Json;
using Sail.Kubernetes.Controller.Services;

namespace Sail.Kubernetes.Controller.Caching;

public class ControllerCache : ICache
{
    private readonly object _sync = new();
    private readonly Dictionary<string, IngressClassData> _ingressClassData = new();
    private readonly Dictionary<string, NamespaceCache> _namespaceCaches = new();
    private readonly SailOptions _options;
    private readonly ILogger<ControllerCache> _logger;

    private bool _isDefaultController;

    public ControllerCache(IOptions<SailOptions> options, ILogger<ControllerCache> logger)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void Update(WatchEventType eventType, V1IngressClass ingressClass)
    {
        if (ingressClass is null)
        {
            throw new ArgumentNullException(nameof(ingressClass));
        }

        if (!string.Equals(_options.ControllerClass, ingressClass.Spec.Controller, StringComparison.OrdinalIgnoreCase))
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
            if (eventType is WatchEventType.Added or WatchEventType.Modified)
            {
                _ingressClassData[ingressClassName] = new IngressClassData(ingressClass);
            }
            else if (eventType == WatchEventType.Deleted)
            {
                _ingressClassData.Remove(ingressClassName);
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
        
        if (IsSailIngress(ingress.Spec))
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

    private bool IsSailIngress(V1IngressSpec spec)
    {
        if (spec.IngressClassName is not null)
        {
            lock (_sync)
            {
                return _ingressClassData.ContainsKey(spec.IngressClassName);
            }
        }

        return _isDefaultController;
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