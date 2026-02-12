using Prometheus;
using Shared.Telemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using Shared.Contracts;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("inference", client =>
{
    client.BaseAddress = new Uri("http://localhost:5285");
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r =>
        r.AddService(TelemetryConstants.ServiceNameDecision))
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
app.UseHttpMetrics();

var fallbackCounter = Metrics.CreateCounter(
    "decision_fallback_total",
    "Number of decisions using baseline fallback");

app.MapPost("/decide", (OrderEvent order) =>
{
    var activity = Activity.Current;
    activity?.SetTag("decision.mode", "baseline");

    // Simple heuristic baseline
    var predictedMinutes = order.ComponentCount * 15;

    fallbackCounter.Inc();

    var decision = new DecisionResult(
        order.OrderId,
        AssignedStation: "Station-A",
        ScheduledStart: DateTime.UtcNow,
        ScheduledEnd: DateTime.UtcNow.AddMinutes(predictedMinutes),
        UsedFallback: true
    );

    return Results.Ok(decision);
});

app.MapHealthChecks("/healthz");
app.MapHealthChecks("/readyz");

app.MapMetrics();

app.Run();
