namespace Planty.Infrastructure.Services;

using Planty.Application.Interfaces;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

public class PermissionService : IPermissionService
{
    private readonly IShareRepository _shareRepository;

    public PermissionService(IShareRepository shareRepository)
    {
        _shareRepository = shareRepository;
    }

    public async Task<ShareRole?> GetUserRoleForPlantAsync(Guid plantId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _shareRepository.GetUserRoleForPlantAsync(plantId, userId, cancellationToken);
    }

    public async Task<ShareRole?> GetUserRoleForLocationAsync(Guid locationId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _shareRepository.GetUserRoleForLocationAsync(locationId, userId, cancellationToken);
    }

    public async Task<bool> CanUserViewPlantAsync(Guid plantId, Guid userId, CancellationToken cancellationToken = default)
    {
        var role = await GetUserRoleForPlantAsync(plantId, userId, cancellationToken);
        return role.HasValue; // Any role can view
    }

    public async Task<bool> CanUserCarePlantAsync(Guid plantId, Guid userId, CancellationToken cancellationToken = default)
    {
        var role = await GetUserRoleForPlantAsync(plantId, userId, cancellationToken);
        return role >= ShareRole.Carer;
    }

    public async Task<bool> CanUserEditPlantAsync(Guid plantId, Guid userId, CancellationToken cancellationToken = default)
    {
        var role = await GetUserRoleForPlantAsync(plantId, userId, cancellationToken);
        return role >= ShareRole.Editor;
    }

    public async Task<bool> CanUserDeletePlantAsync(Guid plantId, Guid userId, CancellationToken cancellationToken = default)
    {
        var role = await GetUserRoleForPlantAsync(plantId, userId, cancellationToken);
        return role == ShareRole.Owner;
    }

    public async Task<bool> CanUserViewLocationAsync(Guid locationId, Guid userId, CancellationToken cancellationToken = default)
    {
        var role = await GetUserRoleForLocationAsync(locationId, userId, cancellationToken);
        return role.HasValue; // Any role can view
    }

    public async Task<bool> CanUserEditLocationAsync(Guid locationId, Guid userId, CancellationToken cancellationToken = default)
    {
        var role = await GetUserRoleForLocationAsync(locationId, userId, cancellationToken);
        return role >= ShareRole.Editor;
    }

    public async Task<bool> CanUserDeleteLocationAsync(Guid locationId, Guid userId, CancellationToken cancellationToken = default)
    {
        var role = await GetUserRoleForLocationAsync(locationId, userId, cancellationToken);
        return role == ShareRole.Owner;
    }

    public async Task<bool> CanUserSharePlantAsync(Guid plantId, Guid userId, CancellationToken cancellationToken = default)
    {
        var role = await GetUserRoleForPlantAsync(plantId, userId, cancellationToken);
        return role == ShareRole.Owner;
    }

    public async Task<bool> CanUserShareLocationAsync(Guid locationId, Guid userId, CancellationToken cancellationToken = default)
    {
        var role = await GetUserRoleForLocationAsync(locationId, userId, cancellationToken);
        return role == ShareRole.Owner;
    }
}
