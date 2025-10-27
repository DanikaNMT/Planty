namespace Planty.Application.Queries.GetLocationById;

using MediatR;
using Planty.Contracts.Locations;
using Planty.Domain.Repositories;

public class GetLocationByIdQueryHandler : IRequestHandler<GetLocationByIdQuery, LocationDetailResponse?>
{
    private readonly ILocationRepository _locationRepository;
    private readonly IShareRepository _shareRepository;

    public GetLocationByIdQueryHandler(ILocationRepository locationRepository, IShareRepository shareRepository)
    {
        _locationRepository = locationRepository;
        _shareRepository = shareRepository;
    }

    public async Task<LocationDetailResponse?> Handle(GetLocationByIdQuery request, CancellationToken cancellationToken)
    {
        var location = await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken);
        
        if (location == null)
        {
            return null;
        }

        var plants = location.Plants?
            .Select(p => new PlantSummary(
                p.Id,
                p.Name,
                p.Species?.Name,
                p.DateAdded
            ))
            .OrderBy(p => p.Name)
            .ToList() ?? new List<PlantSummary>();

        // Check if user owns the location
        if (location.UserId == request.UserId)
        {
            return new LocationDetailResponse(
                location.Id,
                location.Name,
                location.Description,
                location.IsDefault,
                plants,
                IsShared: false,
                UserRole: null,
                OwnerId: null,
                OwnerName: null
            );
        }

        // Check if location is shared with user
        var role = await _shareRepository.GetUserRoleForLocationAsync(request.LocationId, request.UserId, cancellationToken);
        if (!role.HasValue)
        {
            return null; // User doesn't have access
        }

        var roleDto = (Contracts.Shares.ShareRoleDto)role.Value;

        return new LocationDetailResponse(
            location.Id,
            location.Name,
            location.Description,
            location.IsDefault,
            plants,
            IsShared: true,
            UserRole: roleDto,
            OwnerId: location.UserId,
            OwnerName: location.User?.UserName
        );
    }
}
