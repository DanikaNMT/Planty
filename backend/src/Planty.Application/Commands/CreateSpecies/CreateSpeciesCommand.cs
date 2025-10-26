namespace Planty.Application.Commands.CreateSpecies;

using MediatR;
using Planty.Contracts.Species;

public record CreateSpeciesCommand(
    string Name,
    string? Description,
    int? WateringIntervalDays,
    int? FertilizationIntervalDays,
    Guid UserId
) : IRequest<SpeciesResponse>;
