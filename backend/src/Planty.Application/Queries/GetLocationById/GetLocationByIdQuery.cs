namespace Planty.Application.Queries.GetLocationById;

using MediatR;
using Planty.Contracts.Locations;

public record GetLocationByIdQuery(Guid LocationId, Guid UserId) : IRequest<LocationDetailResponse?>;
