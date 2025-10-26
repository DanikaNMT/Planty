namespace Planty.Application.Commands.UpdateSpecies;

using MediatR;
using Planty.Contracts.Species;

public record UpdateSpeciesCommand(
    Guid SpeciesId,
    Guid UserId,
    string Name,
    string? Description,
    int? WateringIntervalDays,
    int? FertilizationIntervalDays
) : IRequest<SpeciesResponse>;
