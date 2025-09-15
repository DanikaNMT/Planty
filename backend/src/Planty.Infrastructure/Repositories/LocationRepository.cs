namespace Planty.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;
using Planty.Infrastructure.Data;

public class LocationRepository : ILocationRepository
{
    private readonly PlantDbContext _context;

    public LocationRepository(PlantDbContext context)
    {
        _context = context;
    }

    public async Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .Include(l => l.Plants)
            .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Location>> GetAllByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Locations
            .Include(l => l.Plants)
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.IsDefault)
            .ThenBy(l => l.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Location> AddAsync(Location location, CancellationToken cancellationToken = default)
    {
        _context.Locations.Add(location);
        await SaveChangesAsync(cancellationToken);
        return location;
    }

    public async Task UpdateAsync(Location location, CancellationToken cancellationToken = default)
    {
        _context.Locations.Update(location);
        await SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var location = await GetByIdAsync(id, cancellationToken);
        if (location != null)
        {
            _context.Locations.Remove(location);
            await SaveChangesAsync(cancellationToken);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}