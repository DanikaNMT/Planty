namespace Planty.Infrastructure.Tests.Repositories;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Planty.Domain.Entities;
using Planty.Infrastructure.Data;
using Planty.Infrastructure.Repositories;

public class WateringRepositoryTests : IDisposable
{
    private readonly PlantDbContext _context;
    private readonly WateringRepository _repository;
    private readonly Guid _userId;
    private readonly Guid _plantId;

    public WateringRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<PlantDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new PlantDbContext(options);
        _repository = new WateringRepository(_context);
        
        _userId = Guid.NewGuid();
        _plantId = Guid.NewGuid();

        // Seed test data
        var user = new User
        {
            Id = _userId,
            UserName = "testuser",
            Email = "test@example.com",
            PasswordHash = "hashedpassword"
        };

        var species = new Species
        {
            Id = Guid.NewGuid(),
            Name = "Test Species",
            UserId = _userId,
            User = user
        };

        var plant = new Plant
        {
            Id = _plantId,
            Name = "Test Plant",
            SpeciesId = species.Id,
            Species = species,
            UserId = _userId,
            User = user
        };

        _context.Users.Add(user);
        _context.Species.Add(species);
        _context.Plants.Add(plant);
        _context.SaveChanges();
    }

    [Fact]
    public async Task AddAsync_ShouldAddWateringToDatabase()
    {
        // Arrange
        var watering = new Watering
        {
            PlantId = _plantId,
            WateredAt = DateTime.UtcNow,
            Notes = "Test watering"
        };

        // Act
        var result = await _repository.AddAsync(watering);
        await _repository.SaveChangesAsync();

        // Assert
        var savedWatering = await _context.Waterings.FirstOrDefaultAsync(w => w.Id == watering.Id);
        savedWatering.Should().NotBeNull();
        savedWatering!.PlantId.Should().Be(_plantId);
        savedWatering.Notes.Should().Be("Test watering");
    }

    [Fact]
    public async Task GetByPlantIdAsync_ShouldReturnWateringsOrderedByDateDescending()
    {
        // Arrange
        var watering1 = new Watering
        {
            PlantId = _plantId,
            WateredAt = DateTime.UtcNow.AddDays(-2),
            Notes = "Two days ago"
        };

        var watering2 = new Watering
        {
            PlantId = _plantId,
            WateredAt = DateTime.UtcNow.AddDays(-1),
            Notes = "Yesterday"
        };

        var watering3 = new Watering
        {
            PlantId = _plantId,
            WateredAt = DateTime.UtcNow,
            Notes = "Today"
        };

        await _context.Waterings.AddRangeAsync(watering1, watering2, watering3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByPlantIdAsync(_plantId);

        // Assert
        var wateringsList = result.ToList();
        wateringsList.Should().HaveCount(3);
        wateringsList[0].Notes.Should().Be("Today");
        wateringsList[1].Notes.Should().Be("Yesterday");
        wateringsList[2].Notes.Should().Be("Two days ago");
    }

    [Fact]
    public async Task GetByPlantIdAsync_ShouldReturnEmptyListWhenNoWaterings()
    {
        // Act
        var result = await _repository.GetByPlantIdAsync(_plantId);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByPlantIdAsync_ShouldOnlyReturnWateringsForSpecificPlant()
    {
        // Arrange
        var otherPlantId = Guid.NewGuid();
        var otherPlant = new Plant
        {
            Id = otherPlantId,
            Name = "Other Plant",
            UserId = _userId
        };
        _context.Plants.Add(otherPlant);
        await _context.SaveChangesAsync();

        var watering1 = new Watering
        {
            PlantId = _plantId,
            WateredAt = DateTime.UtcNow,
            Notes = "For test plant"
        };

        var watering2 = new Watering
        {
            PlantId = otherPlantId,
            WateredAt = DateTime.UtcNow,
            Notes = "For other plant"
        };

        await _context.Waterings.AddRangeAsync(watering1, watering2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByPlantIdAsync(_plantId);

        // Assert
        var wateringsList = result.ToList();
        wateringsList.Should().HaveCount(1);
        wateringsList[0].Notes.Should().Be("For test plant");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
