namespace Planty.Infrastructure.Data;

using Microsoft.EntityFrameworkCore;
using Planty.Domain.Entities;

public class PlantDbContext : DbContext
{
    public PlantDbContext(DbContextOptions<PlantDbContext> options) : base(options) { }


    public DbSet<Plant> Plants { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Location> Locations { get; set; } = null!;
    public DbSet<Watering> Waterings { get; set; } = null!;
    public DbSet<Fertilization> Fertilizations { get; set; } = null!;
    public DbSet<PlantPicture> PlantPictures { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Plant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Species).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.DateAdded).IsRequired();
            entity.Property(e => e.WateringIntervalDays);
            entity.Property(e => e.FertilizationIntervalDays);

            entity.HasOne(e => e.User)
                  .WithMany(u => u.Plants)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Location)
                  .WithMany(l => l.Plants)
                  .HasForeignKey(e => e.LocationId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasMany(e => e.Waterings)
                  .WithOne(w => w.Plant)
                  .HasForeignKey(w => w.PlantId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Fertilizations)
                  .WithOne(f => f.Plant)
                  .HasForeignKey(f => f.PlantId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Pictures)
                  .WithOne(p => p.Plant)
                  .HasForeignKey(p => p.PlantId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Watering>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.WateredAt).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.HasIndex(e => new { e.PlantId, e.WateredAt });
        });

        modelBuilder.Entity<Fertilization>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FertilizedAt).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.HasIndex(e => new { e.PlantId, e.FertilizedAt });
        });

        modelBuilder.Entity<PlantPicture>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TakenAt).IsRequired();
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.HasIndex(e => new { e.PlantId, e.TakenAt });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.UserName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            entity.Property(e => e.PasswordHash).IsRequired();
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsDefault).IsRequired().HasDefaultValue(false);

            entity.HasOne(e => e.User)
                  .WithMany(u => u.Locations)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
