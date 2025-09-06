namespace Planty.API.Tests.Controllers;

using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Planty.Contracts.Plants;
using Planty.Infrastructure.Data;
using System.Net.Http.Json;
using System.Text.Json;

public class PlantsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public PlantsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the real database context
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<PlantDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database for testing
                services.AddDbContext<PlantDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Build the service provider
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database context
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<PlantDbContext>();

                // Ensure the database is created
                db.Database.EnsureCreated();
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetPlants_ReturnsEmptyList_WhenNoPlants()
    {
        // Act
        var response = await _client.GetAsync("/api/plants");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var plants = await response.Content.ReadFromJsonAsync<List<PlantResponse>>();
        plants.Should().NotBeNull();
        plants.Should().BeEmpty();
    }

    [Fact]
    public async Task CreatePlant_ReturnsCreatedPlant()
    {
        // Arrange
        var request = new CreatePlantRequest(
            "Test Plant",
            "Test Species",
            "Test Description",
            7,
            "Test Location"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/plants", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var plant = await response.Content.ReadFromJsonAsync<PlantResponse>();
        plant.Should().NotBeNull();
        plant!.Name.Should().Be(request.Name);
        plant.Species.Should().Be(request.Species);
        plant.Description.Should().Be(request.Description);
        plant.WateringIntervalDays.Should().Be(request.WateringIntervalDays);
        plant.Location.Should().Be(request.Location);
    }

    [Fact]
    public async Task GetPlantById_ReturnsNotFound_WhenPlantDoesNotExist()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/plants/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}