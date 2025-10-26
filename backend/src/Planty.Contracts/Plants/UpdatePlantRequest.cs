namespace Planty.Contracts.Plants;

public record UpdatePlantRequest(
    string Name,
    Guid? SpeciesId,
    string? Description,
    Guid? LocationId
);
