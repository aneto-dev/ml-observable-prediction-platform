namespace Shared.Contracts;

public record DecisionResult(
    string OrderId,
    string AssignedStation,
    DateTime ScheduledStart,
    DateTime ScheduledEnd,
    bool UsedFallback
);
