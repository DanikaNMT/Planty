namespace Planty.Contracts.Plants;

public record PlantResponse(
    Guid Id,
    string Name,
    Guid? SpeciesId,
    string? SpeciesName,
    string? Description,
    DateTime DateAdded,
    DateTime? LastWatered,
    int? WateringIntervalDays,
    Guid? LocationId,
    string? Location,
    string? ImageUrl,
    DateTime? NextWateringDue,
    DateTime? LastFertilized,
    int? FertilizationIntervalDays,
    DateTime? NextFertilizationDue,
    string? LatestPictureUrl
);