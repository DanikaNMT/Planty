namespace Planty.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public ICollection<Plant> Plants { get; set; } = new List<Plant>();
    public ICollection<Location> Locations { get; set; } = new List<Location>();
}
