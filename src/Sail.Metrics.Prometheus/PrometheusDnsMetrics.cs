using Prometheus;
using Yarp.Telemetry.Consumption;

namespace Sail.Metrics.Prometheus;

public class PrometheusDnsMetrics : IMetricsConsumer<NameResolutionMetrics>
{
    private static readonly Counter _dnsLookupsRequested = global::Prometheus.Metrics.CreateCounter(
        "sail_dns_lookups_requested",
        "Number of DNS lookups requested"
    );

    private static readonly Gauge _averageLookupDuration = global::Prometheus.Metrics.CreateGauge(
        "sail_dns_average_lookup_duration",
        "Average DNS lookup duration"
    );

    public void OnMetrics(NameResolutionMetrics previous, NameResolutionMetrics current)
    {
        _dnsLookupsRequested.IncTo(current.DnsLookupsRequested);
        _averageLookupDuration.Set(current.AverageLookupDuration.TotalMilliseconds);
    }
}