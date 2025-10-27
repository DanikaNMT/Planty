namespace Planty.Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;
using Planty.Infrastructure.Data;

public class ShareRepository : IShareRepository
{
    private readonly PlantDbContext _context;

    public ShareRepository(PlantDbContext context)
    {
        _context = context;
    }

    public async Task<Share?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Shares
            .Include(s => s.Owner)
            .Include(s => s.SharedWithUser)
            .Include(s => s.Plant)
            .Include(s => s.Location)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Share>> GetSharesCreatedByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Shares
            .Include(s => s.Owner)
            .Include(s => s.SharedWithUser)
            .Include(s => s.Plant)
            .Include(s => s.Location)
            .Where(s => s.OwnerId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Share>> GetSharesReceivedByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Shares
            .Include(s => s.Owner)
            .Include(s => s.SharedWithUser)
            .Include(s => s.Plant)
            .Include(s => s.Location)
            .Where(s => s.SharedWithUserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Share>> GetPlantSharesForUserAsync(Guid plantId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Shares
            .Include(s => s.SharedWithUser)
            .Where(s => s.PlantId == plantId && s.OwnerId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Share>> GetLocationSharesForUserAsync(Guid locationId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Shares
            .Include(s => s.SharedWithUser)
            .Where(s => s.LocationId == locationId && s.OwnerId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Share?> GetShareForPlantAndUserAsync(Guid plantId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Shares
            .FirstOrDefaultAsync(s => s.PlantId == plantId && s.SharedWithUserId == userId, cancellationToken);
    }

    public async Task<Share?> GetShareForLocationAndUserAsync(Guid locationId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Shares
            .FirstOrDefaultAsync(s => s.LocationId == locationId && s.SharedWithUserId == userId, cancellationToken);
    }

    public async Task<ShareRole?> GetUserRoleForPlantAsync(Guid plantId, Guid userId, CancellationToken cancellationToken = default)
    {
        // First check if user owns the plant
        var plant = await _context.Plants
            .FirstOrDefaultAsync(p => p.Id == plantId, cancellationToken);
        
        if (plant != null && plant.UserId == userId)
        {
            return ShareRole.Owner;
        }

        // Then check direct plant share
        var plantShare = await _context.Shares
            .Where(s => s.PlantId == plantId && s.SharedWithUserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (plantShare != null)
        {
            return plantShare.Role;
        }

        // Finally check if plant is in a shared location
        if (plant?.LocationId != null)
        {
            var locationShare = await _context.Shares
                .Where(s => s.LocationId == plant.LocationId && s.SharedWithUserId == userId)
                .FirstOrDefaultAsync(cancellationToken);

            if (locationShare != null)
            {
                return locationShare.Role;
            }
        }

        return null;
    }

    public async Task<ShareRole?> GetUserRoleForLocationAsync(Guid locationId, Guid userId, CancellationToken cancellationToken = default)
    {
        // First check if user owns the location
        var location = await _context.Locations
            .FirstOrDefaultAsync(l => l.Id == locationId, cancellationToken);
        
        if (location != null && location.UserId == userId)
        {
            return ShareRole.Owner;
        }

        // Then check location share
        var locationShare = await _context.Shares
            .Where(s => s.LocationId == locationId && s.SharedWithUserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        return locationShare?.Role;
    }

    public async Task<IEnumerable<Guid>> GetSharedPlantIdsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // Get directly shared plants
        var directPlantIds = await _context.Shares
            .Where(s => s.ShareType == ShareType.Plant && s.SharedWithUserId == userId)
            .Select(s => s.PlantId!.Value)
            .ToListAsync(cancellationToken);

        // Get plants in shared locations
        var sharedLocationIds = await _context.Shares
            .Where(s => s.ShareType == ShareType.Location && s.SharedWithUserId == userId)
            .Select(s => s.LocationId!.Value)
            .ToListAsync(cancellationToken);

        var locationPlantIds = await _context.Plants
            .Where(p => p.LocationId != null && sharedLocationIds.Contains(p.LocationId.Value))
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        return directPlantIds.Concat(locationPlantIds).Distinct();
    }

    public async Task<IEnumerable<Guid>> GetSharedLocationIdsForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Shares
            .Where(s => s.ShareType == ShareType.Location && s.SharedWithUserId == userId)
            .Select(s => s.LocationId!.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<Share> AddAsync(Share share, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Shares.AddAsync(share, cancellationToken);
        return entry.Entity;
    }

    public Task UpdateAsync(Share share, CancellationToken cancellationToken = default)
    {
        _context.Shares.Update(share);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var share = await GetByIdAsync(id, cancellationToken);
        if (share != null)
        {
            _context.Shares.Remove(share);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
