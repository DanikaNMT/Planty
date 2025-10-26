namespace Planty.Application.Tests.Features.Plants.Queries;

using FluentAssertions;
using Planty.Application.Queries.GetPlantCareHistory;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;
using Moq;
using Xunit;

public class GetPlantCareHistoryQueryHandlerTests
{
    private readonly Mock<IPlantRepository> _plantRepository;
    private readonly Mock<IWateringRepository> _wateringRepository;
    private readonly Mock<IFertilizationRepository> _fertilizationRepository;
    private readonly GetPlantCareHistoryQueryHandler _handler;
    private readonly Guid _userId;

    public GetPlantCareHistoryQueryHandlerTests()
    {
        _plantRepository = new Mock<IPlantRepository>();
        _wateringRepository = new Mock<IWateringRepository>();
        _fertilizationRepository = new Mock<IFertilizationRepository>();
        _handler = new GetPlantCareHistoryQueryHandler(
            _plantRepository.Object, 
            _wateringRepository.Object,
            _fertilizationRepository.Object);
        _userId = Guid.NewGuid();
    }

    [Fact]
    public async Task Handle_ShouldReturnCombinedHistoryOrderedByDate_WhenPlantHasBothTypes()
    {
        // Arrange
        var plantId = Guid.NewGuid();
        var plant = new Plant
        {
            Id = plantId,
            Name = "Test Plant",
            UserId = _userId
        };

        var now = DateTime.UtcNow;
        var waterings = new List<Watering>
        {
            new Watering { Id = Guid.NewGuid(), PlantId = plantId, WateredAt = now.AddDays(-5), Notes = "Watering 5 days ago" },
            new Watering { Id = Guid.NewGuid(), PlantId = plantId, WateredAt = now.AddDays(-2), Notes = "Watering 2 days ago" }
        };

        var fertilizations = new List<Fertilization>
        {
            new Fertilization { Id = Guid.NewGuid(), PlantId = plantId, FertilizedAt = now.AddDays(-4), Notes = "Fertilization 4 days ago" },
            new Fertilization { Id = Guid.NewGuid(), PlantId = plantId, FertilizedAt = now.AddDays(-1), Notes = "Fertilization yesterday" }
        };

        var request = new GetPlantCareHistoryQuery(plantId, _userId);
        _plantRepository.Setup(r => r.GetByIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plant);
        _wateringRepository.Setup(r => r.GetByPlantIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(waterings);
        _fertilizationRepository.Setup(r => r.GetByPlantIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fertilizations);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var historyList = result.ToList();
        historyList.Should().HaveCount(4);
        
        // Verify order (most recent first)
        historyList[0].Type.Should().Be("Fertilization");
        historyList[0].Notes.Should().Be("Fertilization yesterday");
        
        historyList[1].Type.Should().Be("Watering");
        historyList[1].Notes.Should().Be("Watering 2 days ago");
        
        historyList[2].Type.Should().Be("Fertilization");
        historyList[2].Notes.Should().Be("Fertilization 4 days ago");
        
        historyList[3].Type.Should().Be("Watering");
        historyList[3].Notes.Should().Be("Watering 5 days ago");
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlyWaterings_WhenNoFertilizations()
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
            new Watering { Id = Guid.NewGuid(), PlantId = plantId, WateredAt = DateTime.UtcNow, Notes = "Today" }
        };

        var request = new GetPlantCareHistoryQuery(plantId, _userId);
        _plantRepository.Setup(r => r.GetByIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plant);
        _wateringRepository.Setup(r => r.GetByPlantIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(waterings);
        _fertilizationRepository.Setup(r => r.GetByPlantIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Fertilization>());

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var historyList = result.ToList();
        historyList.Should().HaveCount(1);
        historyList[0].Type.Should().Be("Watering");
        historyList[0].Notes.Should().Be("Today");
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlyFertilizations_WhenNoWaterings()
    {
        // Arrange
        var plantId = Guid.NewGuid();
        var plant = new Plant
        {
            Id = plantId,
            Name = "Test Plant",
            UserId = _userId
        };

        var fertilizations = new List<Fertilization>
        {
            new Fertilization { Id = Guid.NewGuid(), PlantId = plantId, FertilizedAt = DateTime.UtcNow, Notes = "Today" }
        };

        var request = new GetPlantCareHistoryQuery(plantId, _userId);
        _plantRepository.Setup(r => r.GetByIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plant);
        _wateringRepository.Setup(r => r.GetByPlantIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Watering>());
        _fertilizationRepository.Setup(r => r.GetByPlantIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fertilizations);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var historyList = result.ToList();
        historyList.Should().HaveCount(1);
        historyList[0].Type.Should().Be("Fertilization");
        historyList[0].Notes.Should().Be("Today");
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoCareEvents()
    {
        // Arrange
        var plantId = Guid.NewGuid();
        var plant = new Plant
        {
            Id = plantId,
            Name = "Test Plant",
            UserId = _userId
        };

        var request = new GetPlantCareHistoryQuery(plantId, _userId);
        _plantRepository.Setup(r => r.GetByIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plant);
        _wateringRepository.Setup(r => r.GetByPlantIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Watering>());
        _fertilizationRepository.Setup(r => r.GetByPlantIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Fertilization>());

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
        var request = new GetPlantCareHistoryQuery(plantId, _userId);
        _plantRepository.Setup(r => r.GetByIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Plant?)null);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _wateringRepository.Verify(r => r.GetByPlantIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _fertilizationRepository.Verify(r => r.GetByPlantIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
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

        var request = new GetPlantCareHistoryQuery(plantId, _userId);
        _plantRepository.Setup(r => r.GetByIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plant);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _wateringRepository.Verify(r => r.GetByPlantIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _fertilizationRepository.Verify(r => r.GetByPlantIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldMapPropertiesCorrectly()
    {
        // Arrange
        var plantId = Guid.NewGuid();
        var plant = new Plant
        {
            Id = plantId,
            Name = "Test Plant",
            UserId = _userId
        };

        var wateringId = Guid.NewGuid();
        var wateringDate = DateTime.UtcNow.AddDays(-1);
        var fertilizationId = Guid.NewGuid();
        var fertilizationDate = DateTime.UtcNow;

        var waterings = new List<Watering>
        {
            new Watering { Id = wateringId, PlantId = plantId, WateredAt = wateringDate, Notes = "Test watering note" }
        };

        var fertilizations = new List<Fertilization>
        {
            new Fertilization { Id = fertilizationId, PlantId = plantId, FertilizedAt = fertilizationDate, Notes = "Test fertilization note" }
        };

        var request = new GetPlantCareHistoryQuery(plantId, _userId);
        _plantRepository.Setup(r => r.GetByIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plant);
        _wateringRepository.Setup(r => r.GetByPlantIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(waterings);
        _fertilizationRepository.Setup(r => r.GetByPlantIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fertilizations);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        var historyList = result.ToList();
        
        var fertilizationEvent = historyList.First(e => e.Type == "Fertilization");
        fertilizationEvent.Id.Should().Be(fertilizationId);
        fertilizationEvent.Timestamp.Should().Be(fertilizationDate);
        fertilizationEvent.Notes.Should().Be("Test fertilization note");

        var wateringEvent = historyList.First(e => e.Type == "Watering");
        wateringEvent.Id.Should().Be(wateringId);
        wateringEvent.Timestamp.Should().Be(wateringDate);
        wateringEvent.Notes.Should().Be("Test watering note");
    }
}
