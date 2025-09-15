namespace Planty.Application.Queries.GetLocations;

using MediatR;
using Planty.Contracts.Locations;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

public class GetLocationsQueryHandler : IRequestHandler<GetLocationsQuery, IEnumerable<LocationResponse>>
{
    private readonly ILocationRepository _locationRepository;

    public GetLocationsQueryHandler(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<IEnumerable<LocationResponse>> Handle(GetLocationsQuery request, CancellationToken cancellationToken)
    {
        var locations = await _locationRepository.GetAllByUserAsync(request.UserId, cancellationToken);

        return locations.Select(MapToResponse);
    }

    private static LocationResponse MapToResponse(Location location)
    {
        return new LocationResponse(
            location.Id,
            location.Name,
            location.Description,
            location.IsDefault,
            location.Plants?.Count ?? 0
        );
    }
}