namespace Planty.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;
using Planty.Infrastructure.Data;

public class FertilizationRepository : IFertilizationRepository
{
    private readonly PlantDbContext _context;

    public FertilizationRepository(PlantDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Fertilization>> GetByPlantIdAsync(Guid plantId, CancellationToken cancellationToken = default)
    {
        return await _context.Fertilizations
            .Where(f => f.PlantId == plantId)
            .OrderByDescending(f => f.FertilizedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Fertilization> AddAsync(Fertilization fertilization, CancellationToken cancellationToken = default)
    {
        await _context.Fertilizations.AddAsync(fertilization, cancellationToken);
        return fertilization;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
