namespace PlantApp.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using PlantApp.Domain.Entities;
using PlantApp.Domain.Repositories;
using PlantApp.Infrastructure.Data;

public class PlantRepository : IPlantRepository
{
    private readonly PlantDbContext _context;

    public PlantRepository(PlantDbContext context)
    {
        _context = context;
    }

    public async Task<Plant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Plants
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Plant>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Plants
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Plant> AddAsync(Plant plant, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Plants.AddAsync(plant, cancellationToken);
        return entry.Entity;
    }

    public Task UpdateAsync(Plant plant, CancellationToken cancellationToken = default)
    {
        _context.Plants.Update(plant);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var plant = await GetByIdAsync(id, cancellationToken);
        if (plant != null)
        {
            _context.Plants.Remove(plant);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}