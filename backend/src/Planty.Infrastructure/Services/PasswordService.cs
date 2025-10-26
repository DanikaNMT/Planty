namespace Planty.Infrastructure.Services;

using Planty.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

public class PasswordService : IPasswordService
{
    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
}
