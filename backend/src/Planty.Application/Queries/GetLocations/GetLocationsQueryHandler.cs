namespace Planty.Application.Queries.GetLocations;

using MediatR;
using Planty.Contracts.Locations;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

public class GetLocationsQueryHandler : IRequestHandler<GetLocationsQuery, IEnumerable<LocationResponse>>
{
    private readonly ILocationRepository _locationRepository;
    private readonly IShareRepository _shareRepository;

    public GetLocationsQueryHandler(ILocationRepository locationRepository, IShareRepository shareRepository)
    {
        _locationRepository = locationRepository;
        _shareRepository = shareRepository;
    }

    public async Task<IEnumerable<LocationResponse>> Handle(GetLocationsQuery request, CancellationToken cancellationToken)
    {
        // Get locations owned by user
        var ownedLocations = await _locationRepository.GetAllByUserAsync(request.UserId, cancellationToken);
        
        // Get shared location IDs
        var sharedLocationIds = await _shareRepository.GetSharedLocationIdsForUserAsync(request.UserId, cancellationToken);
        
        // Build result list
        var result = new List<LocationResponse>();
        
        // Add owned locations (not shared)
        foreach (var location in ownedLocations)
        {
            result.Add(MapToResponse(location, isShared: false));
        }
        
        // Add shared locations with role information
        foreach (var locationId in sharedLocationIds)
        {
            // Skip if already added as owned location
            if (ownedLocations.Any(l => l.Id == locationId))
                continue;
                
            var location = await _locationRepository.GetByIdAsync(locationId, cancellationToken);
            if (location != null)
            {
                // Get user's role for this location
                var role = await _shareRepository.GetUserRoleForLocationAsync(locationId, request.UserId, cancellationToken);
                var roleDto = role.HasValue ? (Contracts.Shares.ShareRoleDto)role.Value : (Contracts.Shares.ShareRoleDto?)null;
                
                result.Add(MapToResponse(
                    location, 
                    isShared: true, 
                    userRole: roleDto,
                    ownerId: location.UserId,
                    ownerName: location.User?.UserName
                ));
            }
        }
        
        return result;
    }

    private static LocationResponse MapToResponse(Location location, bool isShared = false, Contracts.Shares.ShareRoleDto? userRole = null, Guid? ownerId = null, string? ownerName = null)
    {
        return new LocationResponse(
            location.Id,
            location.Name,
            location.Description,
            location.IsDefault,
            location.Plants?.Count ?? 0,
            isShared,
            userRole,
            ownerId,
            ownerName
        );
    }
}