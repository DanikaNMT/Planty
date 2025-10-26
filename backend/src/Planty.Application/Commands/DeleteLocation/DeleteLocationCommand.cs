namespace Planty.Application.Commands.DeleteLocation;

using MediatR;

public record DeleteLocationCommand(
    Guid LocationId,
    Guid UserId
) : IRequest<bool>;
