namespace Planty.Application.Queries.GetLocationById;

using MediatR;
using Planty.Contracts.Locations;
using Planty.Domain.Repositories;

public class GetLocationByIdQueryHandler : IRequestHandler<GetLocationByIdQuery, LocationDetailResponse?>
{
    private readonly ILocationRepository _locationRepository;

    public GetLocationByIdQueryHandler(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<LocationDetailResponse?> Handle(GetLocationByIdQuery request, CancellationToken cancellationToken)
    {
        var location = await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken);
        
        if (location == null || location.UserId != request.UserId)
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

        return new LocationDetailResponse(
            location.Id,
            location.Name,
            location.Description,
            location.IsDefault,
            plants
        );
    }
}
