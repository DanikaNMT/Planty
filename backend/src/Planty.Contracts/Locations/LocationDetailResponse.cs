namespace Planty.Contracts.Locations;

using Planty.Contracts.Plants;
using Planty.Contracts.Shares;

public record LocationDetailResponse(
    Guid Id,
    string Name,
    string? Description,
    bool IsDefault,
    IEnumerable<PlantSummary> Plants,
    bool IsShared,
    ShareRoleDto? UserRole,
    Guid? OwnerId,
    string? OwnerName
);

public record PlantSummary(
    Guid Id,
    string Name,
    string? SpeciesName,
    DateTime AcquiredDate
);
