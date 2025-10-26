namespace Planty.Application.Tests.Features.Plants.Queries;

using FluentAssertions;
using Planty.Application.Queries.GetPlantWaterings;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;
using Moq;
using Xunit;

public class GetPlantWateringsQueryHandlerTests
{
    private readonly Mock<IPlantRepository> _plantRepository;
    private readonly Mock<IWateringRepository> _wateringRepository;
    private readonly GetPlantWateringsQueryHandler _handler;
    private readonly Guid _userId;

    public GetPlantWateringsQueryHandlerTests()
    {
        _plantRepository = new Mock<IPlantRepository>();
        _wateringRepository = new Mock<IWateringRepository>();
        _handler = new GetPlantWateringsQueryHandler(_plantRepository.Object, _wateringRepository.Object);
        _userId = Guid.NewGuid();
    }

    [Fact]
    public async Task Handle_ShouldReturnWateringsOrderedByDate_WhenPlantExists()
    {
        // Arrange
        var plantId = Guid.NewGuid();
        var plant = new Plant
        {
            Id = plantId,
            Name = "Test Plant",
            UserId = _userId
        };

        var waterings = new List<Watering>
        {
            new Watering { Id = Guid.NewGuid(), PlantId = plantId, WateredAt = DateTime.UtcNow.AddDays(-3), Notes = "Three days ago" },
            new Watering { Id = Guid.NewGuid(), PlantId = plantId, WateredAt = DateTime.UtcNow.AddDays(-1), Notes = "Yesterday" },
            new Watering { Id = Guid.NewGuid(), PlantId = plantId, WateredAt = DateTime.UtcNow, Notes = "Today" }
        };

        var request = new GetPlantWateringsQuery(plantId, _userId);
        _plantRepository.Setup(r => r.GetByIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plant);
        _wateringRepository.Setup(r => r.GetByPlantIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(waterings.OrderByDescending(w => w.WateredAt));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var wateringList = result.ToList();
        wateringList.Should().HaveCount(3);
        wateringList[0].Notes.Should().Be("Today");
        wateringList[1].Notes.Should().Be("Yesterday");
        wateringList[2].Notes.Should().Be("Three days ago");
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoWaterings()
    {
        // Arrange
        var plantId = Guid.NewGuid();
        var plant = new Plant
        {
            Id = plantId,
            Name = "Test Plant",
            UserId = _userId
        };

        var request = new GetPlantWateringsQuery(plantId, _userId);
        _plantRepository.Setup(r => r.GetByIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plant);
        _wateringRepository.Setup(r => r.GetByPlantIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Watering>());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenPlantNotFound()
    {
        // Arrange
        var plantId = Guid.NewGuid();
        var request = new GetPlantWateringsQuery(plantId, _userId);
        _plantRepository.Setup(r => r.GetByIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Plant?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _wateringRepository.Verify(r => r.GetByPlantIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenUserDoesNotOwnPlant()
    {
        // Arrange
        var plantId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var plant = new Plant
        {
            Id = plantId,
            Name = "Test Plant",
            UserId = otherUserId
        };

        var request = new GetPlantWateringsQuery(plantId, _userId);
        _plantRepository.Setup(r => r.GetByIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plant);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _wateringRepository.Verify(r => r.GetByPlantIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
