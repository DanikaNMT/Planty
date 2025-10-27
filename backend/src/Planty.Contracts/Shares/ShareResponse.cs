namespace Planty.Contracts.Shares;

public record ShareResponse(
    Guid Id,
    ShareTypeDto ShareType,
    UserInfo Owner,
    UserInfo SharedWithUser,
    Guid? PlantId,
    string? PlantName,
    Guid? LocationId,
    string? LocationName,
    ShareRoleDto Role,
    DateTime CreatedAt
);

public record UserInfo(
    Guid Id,
    string UserName,
    string Email
);
