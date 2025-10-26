namespace Planty.Contracts.Species;

public record UpdateSpeciesRequest(
    string Name,
    string? Description,
    int? WateringIntervalDays,
    int? FertilizationIntervalDays
);
