namespace Planty.Application.Commands.UpdateLocation;

using MediatR;
using Planty.Contracts.Locations;
using Planty.Domain.Repositories;

public class UpdateLocationCommandHandler : IRequestHandler<UpdateLocationCommand, LocationResponse?>
{
    private readonly ILocationRepository _locationRepository;

    public UpdateLocationCommandHandler(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<LocationResponse?> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken);
        
        if (location == null || location.UserId != request.UserId)
        {
            return null;
        }

        location.Name = request.Name;
        location.Description = request.Description;

        await _locationRepository.UpdateAsync(location, cancellationToken);

        return new LocationResponse(
            location.Id,
            location.Name,
            location.Description,
            location.IsDefault,
            location.Plants?.Count ?? 0
        );
    }
}
