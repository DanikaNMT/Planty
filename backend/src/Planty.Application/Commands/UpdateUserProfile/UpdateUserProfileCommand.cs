namespace Planty.Application.Commands.UpdateUserProfile;

using MediatR;
using Planty.Contracts.User;

public record UpdateUserProfileCommand(
    Guid UserId,
    string UserName,
    string Email
) : IRequest<UserProfileResponse>;
