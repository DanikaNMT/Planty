namespace Planty.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Planty.Application.Interfaces;
using Planty.Domain.Entities;
using Planty.Infrastructure.Data;

public class SpeciesRepository : ISpeciesRepository
{
    private readonly PlantDbContext _context;

    public SpeciesRepository(PlantDbContext context)
    {
        _context = context;
    }

    public async Task<Species?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Species
            .Include(s => s.Plants)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Species>> GetAllByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Species
            .Include(s => s.Plants)
            .Where(s => s.UserId == userId)
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Species> AddAsync(Species species, CancellationToken cancellationToken = default)
    {
        _context.Species.Add(species);
        await _context.SaveChangesAsync(cancellationToken);
        return species;
    }

    public async Task UpdateAsync(Species species, CancellationToken cancellationToken = default)
    {
        _context.Species.Update(species);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Species species, CancellationToken cancellationToken = default)
    {
        _context.Species.Remove(species);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Species.AnyAsync(s => s.Id == id, cancellationToken);
    }
}
