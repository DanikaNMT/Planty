namespace Planty.Application.Interfaces;

using Planty.Domain.Entities;

public interface ISpeciesRepository
{
    Task<Species?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Species>> GetAllByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Species> AddAsync(Species species, CancellationToken cancellationToken = default);
    Task UpdateAsync(Species species, CancellationToken cancellationToken = default);
    Task DeleteAsync(Species species, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
