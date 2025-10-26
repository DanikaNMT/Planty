namespace Planty.Application.Commands.UpdateUserProfile;

using MediatR;
using Planty.Contracts.User;
using Planty.Domain.Repositories;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, UserProfileResponse>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserProfileCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserProfileResponse> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {request.UserId} not found.");
        }

        // Check if email is being changed to an email that already exists
        if (user.Email != request.Email)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                throw new InvalidOperationException("Email address is already in use.");
            }
        }

        // Update user properties
        user.UserName = request.UserName;
        user.Email = request.Email;

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return new UserProfileResponse(
            user.Id,
            user.UserName,
            user.Email
        );
    }
}
