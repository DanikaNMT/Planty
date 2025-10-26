namespace Planty.Contracts.User;

public record UpdatePasswordRequest(
    string CurrentPassword,
    string NewPassword
);
