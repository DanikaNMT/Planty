namespace Planty.Application.Commands.CreatePlant;

using MediatR;
using Planty.Contracts.Plants;

public record CreatePlantCommand(
    string Name,
    string Species,
    string? Description,
    int WateringIntervalDays,
    string? Location
) : IRequest<PlantResponse>;