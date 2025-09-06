namespace PlantApp.Application.Tests.Queries;

using FluentAssertions;
using Moq;
using PlantApp.Application.Queries.GetPlants;
using PlantApp.Domain.Entities;
using PlantApp.Domain.Repositories;

public class GetPlantsQueryHandlerTests
{
    private readonly Mock<IPlantRepository> _mockRepository;
    private readonly GetPlantsQueryHandler _handler;

    public GetPlantsQueryHandlerTests()
    {
        _mockRepository = new Mock<IPlantRepository>();
        _handler = new GetPlantsQueryHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAllPlants()
    {
        // Arrange
        var plants = new List<Plant>
        {
            new() { Id = Guid.NewGuid(), Name = "Plant 1", Species = "Species 1", WateringIntervalDays = 7 },
            new() { Id = Guid.NewGuid(), Name = "Plant 2", Species = "Species 2", WateringIntervalDays = 5 }
        };

        _mockRepository
            .Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(plants);

        var query = new GetPlantsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Plant 1");
        result.Should().Contain(p => p.Name == "Plant 2");

        _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}