namespace Planty.Domain.Entities;

public class Plant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Species { get; set; }
    public string? Description { get; set; }
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    public int? WateringIntervalDays { get; set; }
    public string? ImageUrl { get; set; }

    // User relationship
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Location relationship
    public Guid? LocationId { get; set; }
    public Location? Location { get; set; }

    // Watering history
    public ICollection<Watering> Waterings { get; set; } = new List<Watering>();
}