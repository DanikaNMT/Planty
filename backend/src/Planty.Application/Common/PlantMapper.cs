namespace Planty.Application.Common;

using Planty.Contracts.Plants;
using Planty.Contracts.Shares;
using Planty.Domain.Entities;

/// <summary>
/// Helper class to map domain entities to response DTOs
/// </summary>
public static class PlantMapper
{
    public static PlantResponse MapToResponse(Plant plant, bool isShared = false, ShareRoleDto? userRole = null, Guid? ownerId = null, string? ownerName = null)
    {
        var lastWatered = plant.Waterings.OrderByDescending(w => w.WateredAt).FirstOrDefault()?.WateredAt;
        var lastFertilized = plant.Fertilizations.OrderByDescending(f => f.FertilizedAt).FirstOrDefault()?.FertilizedAt;
        var latestPicture = plant.Pictures.OrderByDescending(p => p.TakenAt).FirstOrDefault();
        
        // Get intervals from species if available
        var wateringInterval = plant.Species?.WateringIntervalDays;
        var fertilizationInterval = plant.Species?.FertilizationIntervalDays;
        
        DateTime? nextWateringDue = CalculateNextDue(lastWatered, plant.DateAdded, wateringInterval);
        DateTime? nextFertilizationDue = CalculateNextDue(lastFertilized, plant.DateAdded, fertilizationInterval);

        return new PlantResponse(
            plant.Id,
            plant.Name,
            plant.SpeciesId,
            plant.Species?.Name,
            plant.Description,
            plant.DateAdded,
            lastWatered,
            wateringInterval,
            plant.LocationId,
            plant.Location?.Name,
            plant.ImageUrl,
            nextWateringDue,
            lastFertilized,
            fertilizationInterval,
            nextFertilizationDue,
            latestPicture != null ? $"/api/plants/pictures/{latestPicture.Id}" : null,
            isShared,
            userRole,
            ownerId,
            ownerName
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
