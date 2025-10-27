namespace Planty.Domain.Entities;

public class Fertilization
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime FertilizedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }

    // Plant relationship
    public Guid PlantId { get; set; }
    public Plant Plant { get; set; } = null!;

    // User who performed the fertilization
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
