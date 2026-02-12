namespace Shared.Contracts;

public record PredictionResult(
    string OrderId,
    double PredictedBuildMinutes,
    double UncertaintyScore,
    string ModelVersion,
    string FeatureSchemaVersion,
    bool FallbackUsed
);
