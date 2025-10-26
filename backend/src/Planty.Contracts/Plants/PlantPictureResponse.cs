namespace Planty.Contracts.Plants;

public record PlantPictureResponse(
    Guid Id,
    DateTime TakenAt,
    string Url,
    string? Notes
);
