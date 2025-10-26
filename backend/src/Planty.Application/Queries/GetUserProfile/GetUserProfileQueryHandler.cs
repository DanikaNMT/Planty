namespace Planty.Application.Queries.GetUserProfile;

using MediatR;
using Planty.Contracts.User;
using Planty.Domain.Repositories;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileResponse?>
{
    private readonly IUserRepository _userRepository;

    public GetUserProfileQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserProfileResponse?> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user == null)
        {
            return null;
        }

        return new UserProfileResponse(
            user.Id,
            user.UserName,
            user.Email
        );
    }
}
