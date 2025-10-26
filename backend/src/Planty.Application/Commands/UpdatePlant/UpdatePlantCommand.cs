namespace Planty.Application.Commands.UpdatePlant;

using MediatR;
using Planty.Contracts.Plants;

public record UpdatePlantCommand(
    Guid PlantId,
    Guid UserId,
    string Name,
    string? Species,
    string? Description,
    int? WateringIntervalDays,
    int? FertilizationIntervalDays,
    Guid? LocationId
) : IRequest<PlantResponse>;
