namespace Planty.Contracts.User;

public record UserProfileResponse(
    Guid Id,
    string UserName,
    string Email
);
