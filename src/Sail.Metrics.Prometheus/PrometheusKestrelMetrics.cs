using Prometheus;
using Yarp.Telemetry.Consumption;

namespace Sail.Metrics.Prometheus;

public class PrometheusKestrelMetrics : IMetricsConsumer<KestrelMetrics>
{
    private static readonly Counter _totalConnections = global::Prometheus.Metrics.CreateCounter(
        "sail_kestrel_total_connections",
        "Number of incomming connections opened"
    );

    private static readonly Counter _totalTlsHandshakes = global::Prometheus.Metrics.CreateCounter(
        "sail_kestrel_total_tls_Handshakes",
        "Number of TLS handshakes started"
    );

    private static readonly Gauge _currentTlsHandshakes = global::Prometheus.Metrics.CreateGauge(
        "sail_kestrel_current_tls_handshakes",
        "Number of active TLS handshakes that have started but not yet completed or failed"
    );

    private static readonly Counter _failedTlsHandshakes = global::Prometheus.Metrics.CreateCounter(
        "sail_kestrel_failed_tls_handshakes",
        "Number of TLS handshakes that failed"
    );

    private static readonly Gauge _currentConnections = global::Prometheus.Metrics.CreateGauge(
        "sail_kestrel_current_connections",
        "Number of currently open incomming connections"
    );

    private static readonly Gauge _connectionQueueLength = global::Prometheus.Metrics.CreateGauge(
        "sail_kestrel_connection_queue_length",
        "Number of connections on the queue."
    );

    private static readonly Gauge _requestQueueLength = global::Prometheus.Metrics.CreateGauge(
        "sail_kestrel_request_queue_length",
        "Number of requests on the queue"
    );

    public void OnMetrics(KestrelMetrics previous, KestrelMetrics current)
    {
        _totalConnections.IncTo(current.TotalConnections);
        _totalTlsHandshakes.IncTo(current.TotalTlsHandshakes);
        _currentTlsHandshakes.Set(current.CurrentTlsHandshakes);
        _failedTlsHandshakes.IncTo(current.FailedTlsHandshakes);
        _currentConnections.Set(current.CurrentConnections);
        _connectionQueueLength.Set(current.ConnectionQueueLength);
        _requestQueueLength.Set(current.RequestQueueLength);
    }
}