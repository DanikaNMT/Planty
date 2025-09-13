namespace Planty.Domain.Repositories;

using Planty.Domain.Entities;

public interface IPlantRepository
{
    Task<Plant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Plant>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Plant>> GetAllByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Plant> AddAsync(Plant plant, CancellationToken cancellationToken = default);
    Task UpdateAsync(Plant plant, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}