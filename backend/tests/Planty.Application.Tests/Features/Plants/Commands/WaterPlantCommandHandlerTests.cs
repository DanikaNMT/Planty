namespace Planty.Application.Tests.Features.Plants.Commands;

using FluentAssertions;
using Planty.Application.Commands.WaterPlant;
using Planty.Application.Interfaces;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;
using Moq;
using Xunit;

public class WaterPlantCommandHandlerTests
{
    private readonly Mock<IPlantRepository> _plantRepository;
    private readonly Mock<IWateringRepository> _wateringRepository;
    private readonly Mock<IPermissionService> _permissionService;
    private readonly WaterPlantCommandHandler _handler;
    private readonly Guid _userId;

    public WaterPlantCommandHandlerTests()
    {
        _plantRepository = new Mock<IPlantRepository>();
        _wateringRepository = new Mock<IWateringRepository>();
        _permissionService = new Mock<IPermissionService>();
        _handler = new WaterPlantCommandHandler(_plantRepository.Object, _wateringRepository.Object, _permissionService.Object);
        _userId = Guid.NewGuid();
    }

    [Fact]
    public async Task Handle_ShouldCreateWateringRecord_WhenPlantExists()
    {
        // Arrange
        var plantId = Guid.NewGuid();
        var plant = new Plant
        {
            Id = plantId,
            Name = "Test Plant",
            UserId = _userId
        };
        
        var request = new WaterPlantCommand(plantId, _userId);
        _plantRepository.Setup(r => r.GetByIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plant);
        
        _permissionService.Setup(s => s.CanUserCarePlantAsync(plantId, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        Watering? capturedWatering = null;
        _wateringRepository.Setup(r => r.AddAsync(It.IsAny<Watering>(), It.IsAny<CancellationToken>()))
            .Callback<Watering, CancellationToken>((w, _) => capturedWatering = w)
            .ReturnsAsync((Watering w, CancellationToken _) => w);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        _wateringRepository.Verify(r => r.AddAsync(It.IsAny<Watering>(), It.IsAny<CancellationToken>()), Times.Once);
        _wateringRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        
        capturedWatering.Should().NotBeNull();
        capturedWatering!.PlantId.Should().Be(plantId);
        capturedWatering.WateredAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenPlantNotFound()
    {
        // Arrange
        var plantId = Guid.NewGuid();
        var request = new WaterPlantCommand(plantId, _userId);
        _plantRepository.Setup(r => r.GetByIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Plant?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(request, CancellationToken.None));
        _wateringRepository.Verify(r => r.AddAsync(It.IsAny<Watering>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenUserDoesNotHavePermission()
    {
        // Arrange
        var plantId = Guid.NewGuid();
        var plant = new Plant
        {
            Id = plantId,
            Name = "Test Plant",
            UserId = Guid.NewGuid() // Different user
        };
        
        var request = new WaterPlantCommand(plantId, _userId);
        _plantRepository.Setup(r => r.GetByIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plant);
        
        _permissionService.Setup(s => s.CanUserCarePlantAsync(plantId, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(request, CancellationToken.None));
        _wateringRepository.Verify(r => r.AddAsync(It.IsAny<Watering>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnPlantResponseWithLastWatered_WhenWateringRecordCreated()
    {
        // Arrange
        var plantId = Guid.NewGuid();
        var species = new Species
        {
            Id = Guid.NewGuid(),
            Name = "Test Species",
            WateringIntervalDays = 7,
            UserId = _userId
        };

        var plant = new Plant
        {
            Id = plantId,
            Name = "Test Plant",
            SpeciesId = species.Id,
            Species = species,
            UserId = _userId
        };
        
        var request = new WaterPlantCommand(plantId, _userId);
        _plantRepository.Setup(r => r.GetByIdAsync(plantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(plant);
        
        _permissionService.Setup(s => s.CanUserCarePlantAsync(plantId, _userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        _wateringRepository.Setup(r => r.AddAsync(It.IsAny<Watering>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Watering w, CancellationToken _) => w);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.LastWatered.Should().NotBeNull();
        result.LastWatered.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        result.NextWateringDue.Should().BeCloseTo(DateTime.UtcNow.AddDays(7), TimeSpan.FromSeconds(1));
    }
}
