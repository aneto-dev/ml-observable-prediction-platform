using Prometheus;
using Shared.Contracts;
using Shared.Telemetry;
using System.Diagnostics;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource =>
        resource.AddService(TelemetryConstants.ServiceNameInference))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddOtlpExporter();
    });

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseRouting();

app.UseHttpMetrics(); // Prometheus HTTP metrics

app.MapHealthChecks("/healthz");
app.MapHealthChecks("/readyz");

var predictionCounter = Metrics.CreateCounter(
    TelemetryConstants.MetricFallbackRate,
    "Number of fallback predictions");

var predictionLatency = Metrics.CreateHistogram(
  TelemetryConstants.MetricPredictionLatency,
  "Latency of prediction requests in ms",
  new HistogramConfiguration
  {
      Buckets = Histogram.ExponentialBuckets(5, 2, 8)
  });

app.MapGet("/model", () =>
{
    return Results.Ok(new
    {
        modelVersion = "v0-dev",
        featureSchemaVersion = "v1",
        trainedAtUtc = DateTime.UtcNow,
        mae = 12.3,
        p90Error = 25.7
    });
});

app.MapPost("/predict", (OrderEvent order) =>
{
    var stopwatch = Stopwatch.StartNew();

    var activity = Activity.Current;
    activity?.SetTag("order.id", order.OrderId);
    activity?.SetTag("sku.category", order.SkuCategory);

    // Fake model logic
    var predicted = order.ComponentCount * 12.5;
    var uncertainty = 0.15;

    var fallbackUsed = false;

    var result = new PredictionResult(
        order.OrderId,
        predicted,
        uncertainty,
        ModelVersion: "v0-dev",
        FeatureSchemaVersion: "v1",
        FallbackUsed: fallbackUsed
    );

    stopwatch.Stop();
    predictionLatency.Observe(stopwatch.Elapsed.TotalMilliseconds);

    if (fallbackUsed)
        predictionCounter.Inc();

    return Results.Ok(result);
});

app.MapMetrics(); // Prometheus /metrics endpoint

app.Run();
