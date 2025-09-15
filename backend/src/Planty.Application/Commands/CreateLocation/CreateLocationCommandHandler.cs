namespace Planty.Application.Commands.CreateLocation;

using MediatR;
using Planty.Contracts.Locations;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

public class CreateLocationCommandHandler : IRequestHandler<CreateLocationCommand, LocationResponse>
{
    private readonly ILocationRepository _locationRepository;

    public CreateLocationCommandHandler(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<LocationResponse> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
    {
        var location = new Location
        {
            Name = request.Name,
            Description = request.Description,
            UserId = request.UserId,
            IsDefault = false
        };

        var createdLocation = await _locationRepository.AddAsync(location, cancellationToken);

        return MapToResponse(createdLocation);
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