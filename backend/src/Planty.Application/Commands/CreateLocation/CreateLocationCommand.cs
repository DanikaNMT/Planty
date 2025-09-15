namespace Planty.Application.Commands.CreateLocation;

using MediatR;
using Planty.Contracts.Locations;

public record CreateLocationCommand(
    string Name,
    string? Description,
    Guid UserId
) : IRequest<LocationResponse>;