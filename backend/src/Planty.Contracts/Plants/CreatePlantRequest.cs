namespace Planty.Contracts.Plants;

public record CreatePlantRequest(
    string Name,
    string Species,
    string? Description,
    int WateringIntervalDays,
    Guid? LocationId
);