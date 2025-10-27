namespace Planty.Application.Commands.CreatePlant;

using MediatR;
using Planty.Application.Common;
using Planty.Application.Interfaces;
using Planty.Contracts.Plants;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

public class CreatePlantCommandHandler : IRequestHandler<CreatePlantCommand, PlantResponse>
{
    private readonly IPlantRepository _plantRepository;
    private readonly ISpeciesRepository _speciesRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IPermissionService _permissionService;

    public CreatePlantCommandHandler(
        IPlantRepository plantRepository, 
        ISpeciesRepository speciesRepository,
        ILocationRepository locationRepository,
        IPermissionService permissionService)
    {
        _plantRepository = plantRepository;
        _speciesRepository = speciesRepository;
        _locationRepository = locationRepository;
        _permissionService = permissionService;
    }

    public async Task<PlantResponse> Handle(CreatePlantCommand request, CancellationToken cancellationToken)
    {
        // Validate species if provided
        if (request.SpeciesId.HasValue)
        {
            var species = await _speciesRepository.GetByIdAsync(request.SpeciesId.Value, cancellationToken);
            if (species == null || species.UserId != request.UserId)
            {
                throw new InvalidOperationException("Species not found or does not belong to user");
            }
        }

        // Validate location permission if provided
        if (request.LocationId.HasValue)
        {
            var location = await _locationRepository.GetByIdAsync(request.LocationId.Value, cancellationToken);
            if (location == null)
            {
                throw new InvalidOperationException("Location not found");
            }

            // Check if user has permission to add plants to this location (requires Editor or Owner role)
            if (!await _permissionService.CanUserEditLocationAsync(request.LocationId.Value, request.UserId, cancellationToken))
            {
                throw new UnauthorizedAccessException("You don't have permission to add plants to this location.");
            }
        }

        var plant = new Plant
        {
            Name = request.Name,
            SpeciesId = request.SpeciesId,
            Description = request.Description,
            LocationId = request.LocationId,
            UserId = request.UserId
        };

        var createdPlant = await _plantRepository.AddAsync(plant, cancellationToken);
        await _plantRepository.SaveChangesAsync(cancellationToken);

        // Reload with species included
        var plantWithRelations = await _plantRepository.GetByIdAsync(createdPlant.Id, cancellationToken);
        
        return PlantMapper.MapToResponse(plantWithRelations!);
    }
}
