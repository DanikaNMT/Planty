namespace Planty.Application.Queries.GetSpecies;

using MediatR;
using Planty.Contracts.Species;

public record GetSpeciesQuery(Guid UserId) : IRequest<IEnumerable<SpeciesResponse>>;
