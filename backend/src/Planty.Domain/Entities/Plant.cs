namespace Planty.Domain.Entities;

public class Plant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    public string? ImageUrl { get; set; }

    // User relationship
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Species relationship - determines care intervals
    public Guid? SpeciesId { get; set; }
    public Species? Species { get; set; }

    // Location relationship
    public Guid? LocationId { get; set; }
    public Location? Location { get; set; }

    // Watering history
    public ICollection<Watering> Waterings { get; set; } = new List<Watering>();
    
    // Fertilization history
    public ICollection<Fertilization> Fertilizations { get; set; } = new List<Fertilization>();
    
    // Picture history
    public ICollection<PlantPicture> Pictures { get; set; } = new List<PlantPicture>();
}