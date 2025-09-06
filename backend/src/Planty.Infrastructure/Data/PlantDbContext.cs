namespace Planty.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Planty.Domain.Entities;

public class PlantDbContext : DbContext
{
    public PlantDbContext(DbContextOptions<PlantDbContext> options) : base(options) { }

    public DbSet<Plant> Plants { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Plant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Species).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.DateAdded).IsRequired();
            entity.Property(e => e.WateringIntervalDays).IsRequired();
        });
    }
}
