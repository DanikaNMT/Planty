namespace Planty.Contracts.Plants;

using Planty.Contracts.Shares;

public record PlantResponse(
    Guid Id,
    string Name,
    Guid? SpeciesId,
    string? SpeciesName,
    string? Description,
    DateTime DateAdded,
    DateTime? LastWatered,
    int? WateringIntervalDays,
    Guid? LocationId,
    string? Location,
    string? ImageUrl,
    DateTime? NextWateringDue,
    DateTime? LastFertilized,
    int? FertilizationIntervalDays,
    DateTime? NextFertilizationDue,
    string? LatestPictureUrl,
    bool IsShared,
    ShareRoleDto? UserRole,
    Guid? OwnerId,
    string? OwnerName
);