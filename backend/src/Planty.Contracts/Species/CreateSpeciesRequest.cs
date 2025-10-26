namespace Planty.Contracts.Species;

public record CreateSpeciesRequest(
    string Name,
    string? Description,
    int? WateringIntervalDays,
    int? FertilizationIntervalDays
);
