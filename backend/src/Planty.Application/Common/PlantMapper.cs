namespace Planty.Application.Common;

using Planty.Contracts.Plants;
using Planty.Domain.Entities;

/// <summary>
/// Helper class to map domain entities to response DTOs
/// </summary>
public static class PlantMapper
{
    public static PlantResponse MapToResponse(Plant plant)
    {
        var lastWatered = plant.Waterings.OrderByDescending(w => w.WateredAt).FirstOrDefault()?.WateredAt;
        var lastFertilized = plant.Fertilizations.OrderByDescending(f => f.FertilizedAt).FirstOrDefault()?.FertilizedAt;
        
        DateTime? nextWateringDue = CalculateNextDue(lastWatered, plant.DateAdded, plant.WateringIntervalDays);
        DateTime? nextFertilizationDue = CalculateNextDue(lastFertilized, plant.DateAdded, plant.FertilizationIntervalDays);

        return new PlantResponse(
            plant.Id,
            plant.Name,
            plant.Species,
            plant.Description,
            plant.DateAdded,
            lastWatered,
            plant.WateringIntervalDays,
            plant.Location?.Name,
            plant.ImageUrl,
            nextWateringDue,
            lastFertilized,
            plant.FertilizationIntervalDays,
            nextFertilizationDue
        );
    }

    private static DateTime? CalculateNextDue(DateTime? lastAction, DateTime dateAdded, int? intervalDays)
    {
        if (!intervalDays.HasValue)
            return null;

        if (lastAction.HasValue)
            return lastAction.Value.AddDays(intervalDays.Value);
        
        return dateAdded.AddDays(intervalDays.Value);
    }
}
