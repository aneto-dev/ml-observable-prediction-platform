namespace Shared.Contracts;

public record OrderEvent(
    string OrderId,
    string SkuCategory,
    int ComponentCount,
    bool HasCustomPaint,
    int TechnicianExperienceYears,
    DateTime CreatedAt
);
