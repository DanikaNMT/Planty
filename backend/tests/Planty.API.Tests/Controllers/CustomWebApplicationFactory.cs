using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Data.Sqlite;
using MediatR;
using Planty.Application;
using Planty.Domain.Repositories;
using Planty.Infrastructure.Data;
using Planty.Infrastructure.Repositories;
using Planty.Domain.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace Planty.API.Tests.Controllers;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IDisposable
{
    private SqliteConnection? _connection;
    private User? _testUser;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove the existing DbContext registration
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<PlantDbContext>));
            if (dbContextDescriptor != null)
            {
                services.Remove(dbContextDescriptor);
            }

            var contextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(PlantDbContext));
            if (contextDescriptor != null)
            {
                services.Remove(contextDescriptor);
            }

            // Create and keep connection open for SQLite in-memory database
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            // Use SQLite for testing with the persistent in-memory connection
            services.AddDbContext<PlantDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });
        });

        base.ConfigureWebHost(builder);
    }

    public async Task<User> CreateTestUserAsync()
    {
        if (_testUser != null) return _testUser;

        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PlantDbContext>();

        _testUser = new User
        {
            Id = Guid.NewGuid(),
            UserName = "testuser",
            Email = "test@planty.com",
            PasswordHash = HashPassword("testpassword")
        };

        context.Users.Add(_testUser);
        await context.SaveChangesAsync();
        return _testUser;
    }

    public string GenerateJwtToken(User user)
    {
        var jwtKey = "THIS_IS_A_SUPER_SECRET_KEY_1234567890!!";
        var jwtIssuer = "Planty";
        var jwtAudience = "PlantyUsers";
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = CreateClient();
        var user = await CreateTestUserAsync();
        var token = GenerateJwtToken(user);
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection?.Dispose();
        }
        base.Dispose(disposing);
    }
}