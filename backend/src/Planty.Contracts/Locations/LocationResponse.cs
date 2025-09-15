namespace Planty.Contracts.Locations;

public record LocationResponse(
    Guid Id,
    string Name,
    string? Description,
    bool IsDefault,
    int PlantCount
);