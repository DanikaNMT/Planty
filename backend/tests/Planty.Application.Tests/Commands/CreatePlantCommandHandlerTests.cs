namespace PlantApp.Application.Tests.Commands;

using FluentAssertions;
using Moq;
using PlantApp.Application.Commands.CreatePlant;
using PlantApp.Domain.Entities;
using PlantApp.Domain.Repositories;

public class CreatePlantCommandHandlerTests
{
    private readonly Mock<IPlantRepository> _mockRepository;
    private readonly CreatePlantCommandHandler _handler;

    public CreatePlantCommandHandlerTests()
    {
        _mockRepository = new Mock<IPlantRepository>();
        _handler = new CreatePlantCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsPlantResponse()
    {
        // Arrange
        var command = new CreatePlantCommand(
            "Test Plant",
            "Test Species",
            "Test Description",
            7,
            "Living Room"
        );

        var plant = new Plant
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Species = command.Species,
            Description = command.Description,
            WateringIntervalDays = command.WateringIntervalDays,
            Location = command.Location,
            DateAdded = DateTime.UtcNow
        };

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<Plant>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(plant);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(command.Name);
        result.Species.Should().Be(command.Species);
        result.Description.Should().Be(command.Description);
        result.WateringIntervalDays.Should().Be(command.WateringIntervalDays);
        result.Location.Should().Be(command.Location);

        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Plant>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}