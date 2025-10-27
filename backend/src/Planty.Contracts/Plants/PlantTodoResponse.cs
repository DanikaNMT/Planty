namespace Planty.Contracts.Plants;

using Planty.Contracts.Shares;

public record PlantTodoResponse(
    Guid PlantId,
    string PlantName,
    string? Species,
    string ActionType, // "Water" or "Fertilize"
    DateTime DueDate,
    string? LatestPictureUrl,
    bool IsShared,
    ShareRoleDto? UserRole,
    Guid? OwnerId,
    string? OwnerName
);
