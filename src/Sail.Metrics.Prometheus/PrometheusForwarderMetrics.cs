using Prometheus;
using Yarp.Telemetry.Consumption;

namespace Sail.Metrics.Prometheus;

public class PrometheusForwarderMetrics: IMetricsConsumer<ForwarderMetrics>
{
    private static readonly Counter _requestsStarted = global::Prometheus.Metrics.CreateCounter(
        "sail_proxy_requests_started",
        "Number of requests inititated through the proxy"
    );
    
    private static readonly Counter _requestsFailed = global::Prometheus.Metrics.CreateCounter(
        "sail_proxy_requests_failed",
        "Number of proxy requests that failed"
    );
    
    private static readonly Gauge _currentRequests = global::Prometheus.Metrics.CreateGauge(
        "sail_proxy_current_requests",
        "Number of active proxy requests that have started but not yet completed or failed"
    );
    
    public void OnMetrics(ForwarderMetrics previous, ForwarderMetrics current)
    {
        _requestsStarted.IncTo(current.RequestsStarted);
        _requestsFailed.IncTo(current.RequestsFailed);
        _currentRequests.Set(current.CurrentRequests);
    }
}