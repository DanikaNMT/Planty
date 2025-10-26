namespace Planty.Contracts.Plants;

public record WateringResponse(
    Guid Id,
    DateTime WateredAt,
    string? Notes
);
