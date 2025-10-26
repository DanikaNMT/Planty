namespace Planty.Contracts.Plants;

public record CareEventResponse(
    Guid Id,
    string Type, // "Watering" or "Fertilization"
    DateTime Timestamp,
    string? Notes
);
