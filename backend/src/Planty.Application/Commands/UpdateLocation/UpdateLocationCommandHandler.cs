namespace Planty.Application.Commands.UpdateLocation;

using MediatR;
using Planty.Application.Interfaces;
using Planty.Contracts.Locations;
using Planty.Domain.Repositories;

public class UpdateLocationCommandHandler : IRequestHandler<UpdateLocationCommand, LocationResponse?>
{
    private readonly ILocationRepository _locationRepository;
    private readonly IPermissionService _permissionService;

    public UpdateLocationCommandHandler(ILocationRepository locationRepository, IPermissionService permissionService)
    {
        _locationRepository = locationRepository;
        _permissionService = permissionService;
    }

    public async Task<LocationResponse?> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken);
        
        if (location == null)
        {
            return null;
        }

        // Check if user has permission to edit this location
        if (!await _permissionService.CanUserEditLocationAsync(request.LocationId, request.UserId, cancellationToken))
        {
            throw new UnauthorizedAccessException("You don't have permission to update this location.");
        }

        location.Name = request.Name;
        location.Description = request.Description;

        await _locationRepository.UpdateAsync(location, cancellationToken);

        return new LocationResponse(
            location.Id,
            location.Name,
            location.Description,
            location.IsDefault,
            location.Plants?.Count ?? 0,
            IsShared: false,
            UserRole: null,
            OwnerId: null,
            OwnerName: null
        );
    }
}
