namespace Planty.Contracts.Plants;

public record CareEventResponse(
    Guid Id,
    string Type, // "Watering", "Fertilization", or "Picture"
    DateTime Timestamp,
    string? Notes,
    string? ImageUrl = null // Only populated for Picture type
);
