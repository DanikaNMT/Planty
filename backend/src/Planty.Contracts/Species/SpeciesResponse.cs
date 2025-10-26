namespace Planty.Contracts.Species;

public record SpeciesResponse(
    Guid Id,
    string Name,
    string? Description,
    int? WateringIntervalDays,
    int? FertilizationIntervalDays,
    int PlantCount
);
