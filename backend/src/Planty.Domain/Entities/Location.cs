namespace Planty.Domain.Entities;

public class Location
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    // User relationship
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    // Plants in this location
    public ICollection<Plant> Plants { get; set; } = new List<Plant>();
}