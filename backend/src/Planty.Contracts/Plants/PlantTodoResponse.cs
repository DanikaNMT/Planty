namespace Planty.Contracts.Plants;

public record PlantTodoResponse(
    Guid PlantId,
    string PlantName,
    string? Species,
    string ActionType, // "Water" or "Fertilize"
    DateTime DueDate,
    string? LatestPictureUrl
);
