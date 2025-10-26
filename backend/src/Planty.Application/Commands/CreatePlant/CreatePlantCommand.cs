namespace Planty.Application.Commands.CreatePlant;

using MediatR;
using Planty.Contracts.Plants;

public record CreatePlantCommand(
    string Name,
    Guid? SpeciesId,
    string? Description,
    Guid? LocationId,
    Guid UserId
) : IRequest<PlantResponse>;