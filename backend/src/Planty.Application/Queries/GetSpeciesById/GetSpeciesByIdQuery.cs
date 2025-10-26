namespace Planty.Application.Queries.GetSpeciesById;

using MediatR;
using Planty.Contracts.Species;

public record GetSpeciesByIdQuery(Guid SpeciesId, Guid UserId) : IRequest<SpeciesResponse?>;
