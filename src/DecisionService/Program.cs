using Prometheus;
using Shared.Telemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

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

app.MapHealthChecks("/healthz");
app.MapHealthChecks("/readyz");

app.MapMetrics();

app.Run();
