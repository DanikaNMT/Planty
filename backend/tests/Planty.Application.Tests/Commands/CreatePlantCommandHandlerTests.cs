namespace Planty.Application.Tests.Commands;

using FluentAssertions;
using Moq;
using Planty.Application.Commands.CreatePlant;
using Planty.Application.Interfaces;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

public class CreatePlantCommandHandlerTests
{
    private readonly Mock<IPlantRepository> _mockPlantRepository;
    private readonly Mock<ISpeciesRepository> _mockSpeciesRepository;
    private readonly Mock<ILocationRepository> _mockLocationRepository;
    private readonly Mock<IPermissionService> _mockPermissionService;
    private readonly CreatePlantCommandHandler _handler;

    public CreatePlantCommandHandlerTests()
    {
        _mockPlantRepository = new Mock<IPlantRepository>();
        _mockSpeciesRepository = new Mock<ISpeciesRepository>();
        _mockLocationRepository = new Mock<ILocationRepository>();
        _mockPermissionService = new Mock<IPermissionService>();
        _handler = new CreatePlantCommandHandler(
            _mockPlantRepository.Object, 
            _mockSpeciesRepository.Object,
            _mockLocationRepository.Object,
            _mockPermissionService.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsPlantResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var speciesId = Guid.NewGuid();
        var command = new CreatePlantCommand(
            "Test Plant",
            speciesId,
            "Test Description",
            null, // LocationId
            userId
        );

        var species = new Species
        {
            Id = speciesId,
            Name = "Test Species",
            UserId = userId,
            WateringIntervalDays = 7,
            FertilizationIntervalDays = 30
        };

        var plant = new Plant
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            SpeciesId = command.SpeciesId,
            Species = species,
            Description = command.Description,
            LocationId = command.LocationId,
            DateAdded = DateTime.UtcNow,
            UserId = userId
        };

        _mockSpeciesRepository
            .Setup(r => r.GetByIdAsync(speciesId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(species);

        _mockPlantRepository
            .Setup(r => r.AddAsync(It.IsAny<Plant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(plant);

        _mockPlantRepository
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(plant);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        result.SpeciesId.Should().Be(speciesId);
        result.SpeciesName.Should().Be("Test Species");
        result.Description.Should().Be(command.Description);
        result.Location.Should().BeNull(); // Since LocationId is null, Location name should be null

        _mockPlantRepository.Verify(r => r.AddAsync(It.IsAny<Plant>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockPlantRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}