namespace Planty.Contracts.Locations;

using Planty.Contracts.Plants;

public record LocationDetailResponse(
    Guid Id,
    string Name,
    string? Description,
    bool IsDefault,
    IEnumerable<PlantSummary> Plants
);

public record PlantSummary(
    Guid Id,
    string Name,
    string? SpeciesName,
    DateTime AcquiredDate
);
