namespace Planty.Application.Queries.GetUserProfile;

using MediatR;
using Planty.Contracts.User;

public record GetUserProfileQuery(Guid UserId) : IRequest<UserProfileResponse?>;
