using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Planty.Infrastructure.Data;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Planty.API.Tests.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();

        // Ensure the database is created for each test
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PlantDbContext>();
        context.Database.EnsureCreated();
    }

    [Fact]
    public async Task Register_ReturnsSuccess_WithValidRequest()
    {
        // Arrange
        var request = new
        {
            UserName = "newuser",
            Email = "newuser@planty.com",
            Password = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("Registration successful");
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenEmailAlreadyExists()
    {
        // Arrange
        var request = new
        {
            UserName = "user1",
            Email = "duplicate@planty.com",
            Password = "password123"
        };

        // Register the first user
        await _client.PostAsJsonAsync("/api/auth/register", request);

        // Try to register with the same email
        var duplicateRequest = new
        {
            UserName = "user2",
            Email = "duplicate@planty.com",
            Password = "password456"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", duplicateRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("Email already registered");
    }

    [Fact]
    public async Task Login_ReturnsToken_WithValidCredentials()
    {
        // Arrange - First register a user
        var registerRequest = new
        {
            UserName = "loginuser",
            Email = "login@planty.com",
            Password = "password123"
        };
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();

        var loginRequest = new
        {
            Email = "login@planty.com",
            Password = "password123"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
        var loginResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
        loginResponse.TryGetProperty("token", out var tokenProperty).Should().BeTrue();
        tokenProperty.GetString().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WithInvalidCredentials()
    {
        // Arrange
        var loginRequest = new
        {
            Email = "nonexistent@planty.com",
            Password = "wrongpassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("Invalid credentials");
    }
}