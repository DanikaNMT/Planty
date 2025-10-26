namespace Planty.Application.Commands.UpdateLocation;

using MediatR;
using Planty.Contracts.Locations;

public record UpdateLocationCommand(
    Guid LocationId,
    string Name,
    string? Description,
    Guid UserId
) : IRequest<LocationResponse?>;
