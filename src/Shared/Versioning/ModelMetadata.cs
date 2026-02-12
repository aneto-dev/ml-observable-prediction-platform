namespace Shared.Versioning;

public record ModelMetadata(
    string ModelVersion,
    string FeatureSchemaVersion,
    string TrainingDataRange,
    DateTime TrainedAtUtc,
    double Mae,
    double P90Error
);
