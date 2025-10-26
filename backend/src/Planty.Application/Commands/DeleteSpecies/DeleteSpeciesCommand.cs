namespace Planty.Application.Commands.DeleteSpecies;

using MediatR;

public record DeleteSpeciesCommand(Guid SpeciesId, Guid UserId) : IRequest<Unit>;
