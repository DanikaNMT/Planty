namespace Planty.Domain.Entities;

public enum ShareType
{
    Plant = 0,
    Location = 1
}

public enum ShareRole
{
    Viewer = 0,
    Carer = 1,
    Editor = 2,
    Owner = 3
}

public class Share
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    // Who is sharing
    public Guid OwnerId { get; set; }
    public User Owner { get; set; } = null!;
    
    // Who it's shared with
    public Guid SharedWithUserId { get; set; }
    public User SharedWithUser { get; set; } = null!;
    
    // What is shared
    public ShareType ShareType { get; set; }
    
    // For plant shares
    public Guid? PlantId { get; set; }
    public Plant? Plant { get; set; }
    
    // For location shares
    public Guid? LocationId { get; set; }
    public Location? Location { get; set; }
    
    // Permission level
    public ShareRole Role { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
