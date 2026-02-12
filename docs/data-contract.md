---

```markdown
# Data Contracts

This document defines the shared data contracts used across services.

All contracts live in the `Shared` project to enforce consistency.

---

## OrderEvent

Represents an incoming build order.

```csharp
public record OrderEvent(
    string OrderId,
    string SkuCategory,
    int ComponentCount,
    bool HasCustomPaint,
    int TechnicianExperienceYears,
    DateTime CreatedAt
);
```
