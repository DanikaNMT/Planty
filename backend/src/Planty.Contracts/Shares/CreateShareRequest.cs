namespace Planty.Contracts.Shares;

public record CreateShareRequest(
    ShareTypeDto ShareType,
    Guid? PlantId,
    Guid? LocationId,
    string SharedWithUserEmail,
    ShareRoleDto Role
);
