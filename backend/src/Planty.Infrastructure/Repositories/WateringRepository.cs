namespace Planty.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;
using Planty.Infrastructure.Data;

public class WateringRepository : IWateringRepository
{
    private readonly PlantDbContext _context;

    public WateringRepository(PlantDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Watering>> GetByPlantIdAsync(Guid plantId, CancellationToken cancellationToken = default)
    {
        return await _context.Waterings
            .Include(w => w.User)
            .Where(w => w.PlantId == plantId)
            .OrderByDescending(w => w.WateredAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Watering> AddAsync(Watering watering, CancellationToken cancellationToken = default)
    {
        await _context.Waterings.AddAsync(watering, cancellationToken);
        return watering;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
