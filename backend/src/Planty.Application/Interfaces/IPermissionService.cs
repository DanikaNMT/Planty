namespace Planty.Application.Interfaces;

using Planty.Domain.Entities;

public interface IPermissionService
{
    Task<ShareRole?> GetUserRoleForPlantAsync(Guid plantId, Guid userId, CancellationToken cancellationToken = default);
    Task<ShareRole?> GetUserRoleForLocationAsync(Guid locationId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> CanUserViewPlantAsync(Guid plantId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> CanUserCarePlantAsync(Guid plantId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> CanUserEditPlantAsync(Guid plantId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> CanUserDeletePlantAsync(Guid plantId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> CanUserViewLocationAsync(Guid locationId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> CanUserEditLocationAsync(Guid locationId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> CanUserDeleteLocationAsync(Guid locationId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> CanUserSharePlantAsync(Guid plantId, Guid userId, CancellationToken cancellationToken = default);
    Task<bool> CanUserShareLocationAsync(Guid locationId, Guid userId, CancellationToken cancellationToken = default);
}
