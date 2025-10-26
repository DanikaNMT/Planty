namespace Planty.Application.Commands.UpdatePlant;

using MediatR;
using Planty.Contracts.Plants;

public record UpdatePlantCommand(
    Guid PlantId,
    Guid UserId,
    string Name,
    Guid? SpeciesId,
    string? Description,
    Guid? LocationId
) : IRequest<PlantResponse>;
