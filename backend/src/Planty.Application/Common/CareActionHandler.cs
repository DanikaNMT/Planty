namespace Planty.Application.Common;

using System;
using System.Threading;
using System.Threading.Tasks;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

/// <summary>
/// Base class for care action handlers (watering, fertilization, etc.) to reduce code duplication
/// </summary>
public abstract class CareActionHandler<TCareAction> where TCareAction : class
{
    private readonly IPlantRepository _plantRepository;

    protected CareActionHandler(IPlantRepository plantRepository)
    {
        _plantRepository = plantRepository;
    }

    /// <summary>
    /// Validates that the plant exists and belongs to the user
    /// </summary>
    protected async Task<Plant> ValidatePlantOwnershipAsync(Guid plantId, Guid userId, CancellationToken cancellationToken)
    {
        var plant = await _plantRepository.GetByIdAsync(plantId, cancellationToken);
        if (plant == null || plant.UserId != userId)
            throw new Exception($"Plant with id {plantId} not found for this user");
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
