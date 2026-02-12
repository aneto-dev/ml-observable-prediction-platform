namespace Shared.Telemetry;

public static class TelemetryConstants
{
    public const string ServiceNameInference = "inference-api";
    public const string ServiceNameDecision = "decision-service";
    public const string ServiceNameMonitoring = "monitoring-worker";

    public const string MetricPredictionLatency = "prediction_latency_ms";
    public const string MetricFallbackRate = "prediction_fallback_total";
    public const string MetricModelMae = "model_mae";
    public const string MetricDriftScore = "feature_drift_score";
}
