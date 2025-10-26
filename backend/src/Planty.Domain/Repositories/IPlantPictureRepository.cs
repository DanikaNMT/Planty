namespace Planty.Domain.Repositories;

using Planty.Domain.Entities;

public interface IPlantPictureRepository
{
    Task<IEnumerable<PlantPicture>> GetByPlantIdAsync(Guid plantId, CancellationToken cancellationToken = default);
    Task<PlantPicture?> GetLatestByPlantIdAsync(Guid plantId, CancellationToken cancellationToken = default);
    Task<PlantPicture?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(PlantPicture picture, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
