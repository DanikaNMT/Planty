namespace Planty.Domain.Repositories;

using Planty.Domain.Entities;

public interface IWateringRepository
{
    Task<IEnumerable<Watering>> GetByPlantIdAsync(Guid plantId, CancellationToken cancellationToken = default);
    Task<Watering> AddAsync(Watering watering, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
