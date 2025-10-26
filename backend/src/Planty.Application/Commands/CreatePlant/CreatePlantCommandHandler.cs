namespace Planty.Application.Commands.CreatePlant;

using MediatR;
using Planty.Contracts.Plants;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

public class CreatePlantCommandHandler : IRequestHandler<CreatePlantCommand, PlantResponse>
{
    private readonly IPlantRepository _plantRepository;

    public CreatePlantCommandHandler(IPlantRepository plantRepository)
    {
        _plantRepository = plantRepository;
    }

    public async Task<PlantResponse> Handle(CreatePlantCommand request, CancellationToken cancellationToken)
    {
        var plant = new Plant
        {
            Name = request.Name,
            Species = request.Species,
            Description = request.Description,
            WateringIntervalDays = request.WateringIntervalDays,
            LocationId = request.LocationId,
            UserId = request.UserId
        };

        var createdPlant = await _plantRepository.AddAsync(plant, cancellationToken);
        await _plantRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(createdPlant);
    }

    private static PlantResponse MapToResponse(Plant plant)
    {
        var lastWatered = plant.Waterings.OrderByDescending(w => w.WateredAt).FirstOrDefault()?.WateredAt;
        
        DateTime? nextWateringDue = null;
        if (plant.WateringIntervalDays.HasValue && lastWatered.HasValue)
        {
            nextWateringDue = lastWatered.Value.AddDays(plant.WateringIntervalDays.Value);
        }
        else if (plant.WateringIntervalDays.HasValue)
        {
            nextWateringDue = plant.DateAdded.AddDays(plant.WateringIntervalDays.Value);
        }

        return new PlantResponse(
            plant.Id,
            plant.Name,
            plant.Species,
            plant.Description,
            plant.DateAdded,
            lastWatered,
            plant.WateringIntervalDays,
            plant.Location?.Name,
            plant.ImageUrl,
            nextWateringDue
        );
    }
}
