namespace Planty.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;
using Planty.Infrastructure.Data;

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
            .Include(p => p.Location)
            .Include(p => p.Waterings.OrderByDescending(w => w.WateredAt).Take(1))
            .Include(p => p.Fertilizations.OrderByDescending(f => f.FertilizedAt).Take(1))
            .Include(p => p.Pictures.OrderByDescending(pic => pic.TakenAt).Take(1))
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }


    public async Task<IEnumerable<Plant>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Plants
            .Include(p => p.Location)
            .Include(p => p.Waterings.OrderByDescending(w => w.WateredAt).Take(1))
            .Include(p => p.Fertilizations.OrderByDescending(f => f.FertilizedAt).Take(1))
            .Include(p => p.Pictures.OrderByDescending(pic => pic.TakenAt).Take(1))
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Plant>> GetAllByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Plants
            .Include(p => p.Location)
            .Include(p => p.Waterings.OrderByDescending(w => w.WateredAt).Take(1))
            .Include(p => p.Fertilizations.OrderByDescending(f => f.FertilizedAt).Take(1))
            .Include(p => p.Pictures.OrderByDescending(pic => pic.TakenAt).Take(1))
            .Where(p => p.UserId == userId)
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