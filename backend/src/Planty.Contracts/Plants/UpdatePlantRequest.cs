namespace Planty.Contracts.Plants;

public record UpdatePlantRequest(
    string Name,
    string? Species,
    string? Description,
    int? WateringIntervalDays,
    int? FertilizationIntervalDays,
    Guid? LocationId
);
