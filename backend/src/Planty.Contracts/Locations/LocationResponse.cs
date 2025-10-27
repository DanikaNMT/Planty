namespace Planty.Contracts.Locations;

using Planty.Contracts.Shares;

public record LocationResponse(
    Guid Id,
    string Name,
    string? Description,
    bool IsDefault,
    int PlantCount,
    bool IsShared,
    ShareRoleDto? UserRole,
    Guid? OwnerId,
    string? OwnerName
);