using System.Net.Http.Json;
using FluentAssertions;
using Planty.Contracts.Plants;
using Xunit;

namespace Planty.API.Tests.Controllers
{
    public partial class PlantsControllerTests
    {
        [Fact]
        public async Task WaterPlant_ReturnsOkAndUpdatesLastWatered()
        {
            // Arrange: create a plant
            var request = new CreatePlantRequest(
                "Watered Plant",
                "Species",
                "Desc",
                3,
                "Location"
            );
            var createResponse = await _client.PostAsJsonAsync("/api/plants", request);
            createResponse.EnsureSuccessStatusCode();
            var plant = await createResponse.Content.ReadFromJsonAsync<PlantResponse>();
            plant.Should().NotBeNull();

            // Act: water the plant
            var waterResponse = await _client.PostAsync($"/api/plants/{plant!.Id}/water", null);
            waterResponse.EnsureSuccessStatusCode();
            var wateredPlant = await waterResponse.Content.ReadFromJsonAsync<PlantResponse>();

            // Assert
            wateredPlant.Should().NotBeNull();
            wateredPlant!.LastWatered.Should().NotBeNull();
            wateredPlant.LastWatered.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
        }

        [Fact]
        public async Task WaterPlant_ReturnsNotFound_ForInvalidId()
        {
            // Act
            var response = await _client.PostAsync($"/api/plants/{Guid.NewGuid()}/water", null);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
    }
}
