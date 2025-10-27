namespace Planty.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public ICollection<Plant> Plants { get; set; } = new List<Plant>();
    public ICollection<Location> Locations { get; set; } = new List<Location>();
    public ICollection<Species> Species { get; set; } = new List<Species>();
    
    // Shares created by this user
    public ICollection<Share> SharesCreated { get; set; } = new List<Share>();
    
    // Shares received by this user
    public ICollection<Share> SharesReceived { get; set; } = new List<Share>();

    // Actions performed by this user
    public ICollection<Watering> Waterings { get; set; } = new List<Watering>();
    public ICollection<Fertilization> Fertilizations { get; set; } = new List<Fertilization>();
    public ICollection<PlantPicture> Pictures { get; set; } = new List<PlantPicture>();
}
