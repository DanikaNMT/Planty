namespace Planty.Application.Commands.UpdatePassword;

using MediatR;
using Planty.Application.Interfaces;
using Planty.Domain.Repositories;

public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;

    public UpdatePasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
    }

    public async Task<Unit> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user == null)
        {
            throw new InvalidOperationException($"User with ID {request.UserId} not found.");
        }

        // Verify current password
        if (!_passwordService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            throw new InvalidOperationException("Current password is incorrect.");
        }

        // Hash and update password
        user.PasswordHash = _passwordService.HashPassword(request.NewPassword);

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
