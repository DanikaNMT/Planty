namespace Planty.Application.Common;

using System;
using System.Threading;
using System.Threading.Tasks;
using Planty.Application.Interfaces;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

/// <summary>
/// Base class for care action handlers (watering, fertilization, etc.) to reduce code duplication
/// </summary>
public abstract class CareActionHandler<TCareAction> where TCareAction : class
{
    private readonly IPlantRepository _plantRepository;
    private readonly IPermissionService _permissionService;

    protected CareActionHandler(IPlantRepository plantRepository, IPermissionService permissionService)
    {
        _plantRepository = plantRepository;
        _permissionService = permissionService;
    }

    /// <summary>
    /// Validates that the plant exists and user has permission to perform care actions
    /// </summary>
    protected async Task<Plant> ValidatePlantCarePermissionAsync(Guid plantId, Guid userId, CancellationToken cancellationToken)
    {
        var plant = await _plantRepository.GetByIdAsync(plantId, cancellationToken);
        if (plant == null)
            throw new InvalidOperationException($"Plant with id {plantId} not found");
        
        if (!await _permissionService.CanUserCarePlantAsync(plantId, userId, cancellationToken))
            throw new UnauthorizedAccessException("You don't have permission to perform care actions on this plant.");
        
        return plant;
    }

    /// <summary>
    /// Calculates the next care action due date based on the interval
    /// </summary>
    protected DateTime? CalculateNextCareActionDue(DateTime actionDate, int? intervalDays)
    {
        return intervalDays.HasValue ? actionDate.AddDays(intervalDays.Value) : null;
    }
}
