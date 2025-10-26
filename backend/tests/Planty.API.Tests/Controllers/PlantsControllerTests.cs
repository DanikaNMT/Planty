namespace Planty.API.Tests.Controllers;

using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Planty.Contracts.Plants;
using Planty.Infrastructure.Data;
using System.Net.Http.Json;
using System.Text.Json;

public partial class PlantsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public PlantsControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();

        // Ensure the database is created and seeded for each test
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PlantDbContext>();
        context.Database.EnsureCreated();
    }

    [Fact]
    public async Task GetPlants_ReturnsEmptyList_WhenNoPlants()
    {
        // Arrange
        var authenticatedClient = await _factory.CreateAuthenticatedClientAsync();
        
        // Act
        var response = await authenticatedClient.GetAsync("/api/plants");
        
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
        var authenticatedClient = await _factory.CreateAuthenticatedClientAsync();
        var request = new CreatePlantRequest(
            "Test Plant",
            "Test Species",
            "Test Description",
            7,
            null // LocationId
        );

        // Act
        var response = await authenticatedClient.PostAsJsonAsync("/api/plants", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var plant = await response.Content.ReadFromJsonAsync<PlantResponse>();
        plant.Should().NotBeNull();
        plant!.Name.Should().Be(request.Name);
        plant.Species.Should().Be(request.Species);
        plant.Description.Should().Be(request.Description);
        plant.WateringIntervalDays.Should().Be(request.WateringIntervalDays);
        plant.Location.Should().BeNull(); // Since LocationId is null, Location name should be null
    }

    [Fact]
    public async Task CreatePlant_WithOnlyName_ReturnsCreatedPlant()
    {
        // Arrange
        var authenticatedClient = await _factory.CreateAuthenticatedClientAsync();
        var request = new CreatePlantRequest(
            "My Plant",
            null, // Species
            null, // Description
            null, // WateringIntervalDays
            null  // LocationId
        );

        // Act
        var response = await authenticatedClient.PostAsJsonAsync("/api/plants", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var plant = await response.Content.ReadFromJsonAsync<PlantResponse>();
        plant.Should().NotBeNull();
        plant!.Name.Should().Be(request.Name);
        plant.Species.Should().BeNull();
        plant.Description.Should().BeNull();
        plant.WateringIntervalDays.Should().BeNull();
        plant.Location.Should().BeNull();
        plant.NextWateringDue.Should().BeNull(); // No watering interval set
    }

    [Fact]
    public async Task GetPlantById_ReturnsNotFound_WhenPlantDoesNotExist()
    {
        // Arrange
        var authenticatedClient = await _factory.CreateAuthenticatedClientAsync();
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await authenticatedClient.GetAsync($"/api/plants/{nonExistentId}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }
}