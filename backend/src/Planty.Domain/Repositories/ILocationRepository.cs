namespace Planty.Domain.Repositories;

using Planty.Domain.Entities;

public interface ILocationRepository
{
    Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Location>> GetAllByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Location> AddAsync(Location location, CancellationToken cancellationToken = default);
    Task UpdateAsync(Location location, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}