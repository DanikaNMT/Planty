namespace Planty.Application.Tests.Queries;

using FluentAssertions;
using Moq;
using Planty.Application.Queries.GetPlants;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

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
        var userId = Guid.NewGuid();
        var species1 = new Species { Id = Guid.NewGuid(), Name = "Species 1", WateringIntervalDays = 7, UserId = userId };
        var species2 = new Species { Id = Guid.NewGuid(), Name = "Species 2", WateringIntervalDays = 5, UserId = userId };
        
        var plants = new List<Plant>
        {
            new() { Id = Guid.NewGuid(), Name = "Plant 1", Species = species1, SpeciesId = species1.Id, UserId = userId },
            new() { Id = Guid.NewGuid(), Name = "Plant 2", Species = species2, SpeciesId = species2.Id, UserId = userId }
        };

        _mockRepository
            .Setup(r => r.GetAllByUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plants);

        var query = new GetPlantsQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(p => p.Name == "Plant 1");
        result.Should().Contain(p => p.Name == "Plant 2");

        _mockRepository.Verify(r => r.GetAllByUserAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
    }
}