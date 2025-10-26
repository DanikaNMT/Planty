namespace Planty.Contracts.User;

public record UpdateUserProfileRequest(
    string UserName,
    string Email
);
