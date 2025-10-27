namespace Planty.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;
using Planty.Infrastructure.Data;

public class PlantPictureRepository : IPlantPictureRepository
{
    private readonly PlantDbContext _context;

    public PlantPictureRepository(PlantDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PlantPicture>> GetByPlantIdAsync(Guid plantId, CancellationToken cancellationToken = default)
    {
        return await _context.PlantPictures
            .Include(p => p.User)
            .Where(p => p.PlantId == plantId)
            .OrderByDescending(p => p.TakenAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<PlantPicture?> GetLatestByPlantIdAsync(Guid plantId, CancellationToken cancellationToken = default)
    {
        return await _context.PlantPictures
            .Include(p => p.User)
            .Where(p => p.PlantId == plantId)
            .OrderByDescending(p => p.TakenAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PlantPicture?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.PlantPictures
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task AddAsync(PlantPicture picture, CancellationToken cancellationToken = default)
    {
        await _context.PlantPictures.AddAsync(picture, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
