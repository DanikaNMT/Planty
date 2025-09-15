namespace Planty.Application.Queries.GetLocations;

using MediatR;
using Planty.Contracts.Locations;

public record GetLocationsQuery(Guid UserId) : IRequest<IEnumerable<LocationResponse>>;