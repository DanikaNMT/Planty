namespace Planty.Contracts.Plants;

public record CreatePlantRequest(
    string Name,
    Guid? SpeciesId,
    string? Description,
    Guid? LocationId
);