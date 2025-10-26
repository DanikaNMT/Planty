namespace Planty.Application.Commands.UpdatePlant;

using MediatR;
using Planty.Application.Common;
using Planty.Contracts.Plants;
using Planty.Domain.Repositories;

public class UpdatePlantCommandHandler : IRequestHandler<UpdatePlantCommand, PlantResponse>
{
    private readonly IPlantRepository _plantRepository;
    private readonly ILocationRepository _locationRepository;

    public UpdatePlantCommandHandler(
        IPlantRepository plantRepository,
        ILocationRepository locationRepository)
    {
        _plantRepository = plantRepository;
        _locationRepository = locationRepository;
    }

    public async Task<PlantResponse> Handle(UpdatePlantCommand request, CancellationToken cancellationToken)
    {
        var plant = await _plantRepository.GetByIdAsync(request.PlantId, cancellationToken);
        
        if (plant == null)
        {
            throw new InvalidOperationException($"Plant with ID {request.PlantId} not found.");
        }

        if (plant.UserId != request.UserId)
        {
            throw new UnauthorizedAccessException("You don't have permission to update this plant.");
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
        plant.Species = request.Species;
        plant.Description = request.Description;
        plant.WateringIntervalDays = request.WateringIntervalDays;
        plant.FertilizationIntervalDays = request.FertilizationIntervalDays;
        plant.LocationId = request.LocationId;

        await _plantRepository.UpdateAsync(plant, cancellationToken);
        await _plantRepository.SaveChangesAsync(cancellationToken);

        // Reload plant with all related data
        var updatedPlant = await _plantRepository.GetByIdAsync(plant.Id, cancellationToken);
        return PlantMapper.MapToResponse(updatedPlant!);
    }
}
