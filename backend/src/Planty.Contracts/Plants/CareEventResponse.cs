namespace Planty.Contracts.Plants;

public record CareEventResponse(
    Guid Id,
    string Type, // "Watering", "Fertilization", or "Picture"
    DateTime Timestamp,
    string? Notes,
    string? ImageUrl, // Only populated for Picture type
    Guid UserId,
    string UserName
);
