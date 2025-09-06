namespace Planty.Domain.Entities;

public class Plant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    public DateTime? LastWatered { get; set; }
    public int WateringIntervalDays { get; set; } = 7;
    public string? Location { get; set; }
    public string? ImageUrl { get; set; }
}