namespace Planty.Application.Commands.UpdatePassword;

using MediatR;

public record UpdatePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword
) : IRequest<Unit>;
