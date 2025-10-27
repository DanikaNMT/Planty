namespace Planty.Application.Commands.DeleteShare;

using MediatR;

public record DeleteShareCommand(
    Guid ShareId,
    Guid UserId
) : IRequest<Unit>;
