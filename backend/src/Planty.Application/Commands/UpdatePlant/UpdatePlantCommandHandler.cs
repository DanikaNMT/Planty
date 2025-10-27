namespace Planty.Application.Commands.UpdatePlant;

using MediatR;
using Planty.Application.Common;
using Planty.Application.Interfaces;
using Planty.Contracts.Plants;
using Planty.Domain.Repositories;

public class UpdatePlantCommandHandler : IRequestHandler<UpdatePlantCommand, PlantResponse>
{
    private readonly IPlantRepository _plantRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IPermissionService _permissionService;

    public UpdatePlantCommandHandler(
        IPlantRepository plantRepository,
        ILocationRepository locationRepository,
        ISpeciesRepository speciesRepository,
        IPermissionService permissionService)
    {
        _plantRepository = plantRepository;
        _locationRepository = locationRepository;
        _speciesRepository = speciesRepository;
        _permissionService = permissionService;
    }

    public async Task<PlantResponse> Handle(UpdatePlantCommand request, CancellationToken cancellationToken)
    {
        var plant = await _plantRepository.GetByIdAsync(request.PlantId, cancellationToken);
        
        if (plant == null)
        {
            throw new InvalidOperationException($"Plant with ID {request.PlantId} not found.");
        }

        // Check if user has permission to edit this plant
        if (!await _permissionService.CanUserEditPlantAsync(plant.Id, request.UserId, cancellationToken))
        {
            throw new UnauthorizedAccessException("You don't have permission to update this plant.");
        }

        // Validate species if provided
        if (request.SpeciesId.HasValue)
        {
            var species = await _speciesRepository.GetByIdAsync(request.SpeciesId.Value, cancellationToken);
            if (species == null || species.UserId != request.UserId)
            {
                throw new InvalidOperationException($"Species with ID {request.SpeciesId} not found or doesn't belong to user.");
            }
        }

        // Validate location if provided
        if (request.LocationId.HasValue)
        {
            var location = await _locationRepository.GetByIdAsync(request.LocationId.Value, cancellationToken);
            if (location == null || location.UserId != request.UserId)
            {
                throw new InvalidOperationException($"Location with ID {request.LocationId} not found or doesn't belong to user.");
            }
        }

        // Update plant properties
        plant.Name = request.Name;
        plant.SpeciesId = request.SpeciesId;
        plant.Description = request.Description;
        plant.LocationId = request.LocationId;

        await _plantRepository.UpdateAsync(plant, cancellationToken);
        await _plantRepository.SaveChangesAsync(cancellationToken);

        // Reload plant with all related data
        var updatedPlant = await _plantRepository.GetByIdAsync(plant.Id, cancellationToken);
        return PlantMapper.MapToResponse(updatedPlant!);
    }
}
