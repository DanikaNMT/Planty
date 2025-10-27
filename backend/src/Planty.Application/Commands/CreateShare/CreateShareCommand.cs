namespace Planty.Application.Commands.CreateShare;

using MediatR;
using Planty.Contracts.Shares;

public record CreateShareCommand(
    Guid OwnerId,
    ShareTypeDto ShareType,
    Guid? PlantId,
    Guid? LocationId,
    string SharedWithUserEmail,
    ShareRoleDto Role
) : IRequest<ShareResponse>;
