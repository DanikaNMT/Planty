namespace Planty.Domain.Entities;

public class PlantPicture
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime TakenAt { get; set; } = DateTime.UtcNow;
    public string FilePath { get; set; } = string.Empty;
    public string? Notes { get; set; }

    // Plant relationship
    public Guid PlantId { get; set; }
    public Plant Plant { get; set; } = null!;
}
