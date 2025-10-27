namespace Planty.Application.Commands.UpdateShare;

using MediatR;
using Planty.Contracts.Shares;

public record UpdateShareCommand(
    Guid ShareId,
    Guid UserId,
    ShareRoleDto Role
) : IRequest<ShareResponse>;
