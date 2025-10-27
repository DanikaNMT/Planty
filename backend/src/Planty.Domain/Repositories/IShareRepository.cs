namespace Planty.Domain.Repositories;

using Planty.Domain.Entities;

public interface IShareRepository
{
    Task<Share?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Share>> GetSharesCreatedByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Share>> GetSharesReceivedByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Share>> GetPlantSharesForUserAsync(Guid plantId, Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Share>> GetLocationSharesForUserAsync(Guid locationId, Guid userId, CancellationToken cancellationToken = default);
    Task<Share?> GetShareForPlantAndUserAsync(Guid plantId, Guid userId, CancellationToken cancellationToken = default);
    Task<Share?> GetShareForLocationAndUserAsync(Guid locationId, Guid userId, CancellationToken cancellationToken = default);
    Task<ShareRole?> GetUserRoleForPlantAsync(Guid plantId, Guid userId, CancellationToken cancellationToken = default);
    Task<ShareRole?> GetUserRoleForLocationAsync(Guid locationId, Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Guid>> GetSharedPlantIdsForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Guid>> GetSharedLocationIdsForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Share> AddAsync(Share share, CancellationToken cancellationToken = default);
    Task UpdateAsync(Share share, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
