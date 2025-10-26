namespace Planty.Domain.Repositories;

using Planty.Domain.Entities;

public interface IFertilizationRepository
{
    Task<IEnumerable<Fertilization>> GetByPlantIdAsync(Guid plantId, CancellationToken cancellationToken = default);
    Task<Fertilization> AddAsync(Fertilization fertilization, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
