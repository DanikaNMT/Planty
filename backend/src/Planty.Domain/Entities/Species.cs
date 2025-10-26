namespace Planty.Domain.Entities;

public class Species
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? WateringIntervalDays { get; set; }
    public int? FertilizationIntervalDays { get; set; }

    // User relationship - each user has their own species catalog
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Plants of this species
    public ICollection<Plant> Plants { get; set; } = new List<Plant>();
}
