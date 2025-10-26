namespace Planty.Contracts.Plants;

public record PlantResponse(
    Guid Id,
    string Name,
    string? Species,
    string? Description,
    DateTime DateAdded,
    DateTime? LastWatered,
    int? WateringIntervalDays,
    string? Location,
    string? ImageUrl,
    DateTime? NextWateringDue,
    DateTime? LastFertilized,
    int? FertilizationIntervalDays,
    DateTime? NextFertilizationDue
);