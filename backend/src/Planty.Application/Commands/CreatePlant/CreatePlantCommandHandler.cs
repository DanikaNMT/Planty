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
            Location = request.Location
        };

        var createdPlant = await _plantRepository.AddAsync(plant, cancellationToken);
        await _plantRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(createdPlant);
    }

    private static PlantResponse MapToResponse(Plant plant)
    {
        var nextWateringDue = plant.LastWatered?.AddDays(plant.WateringIntervalDays) ?? 
                             plant.DateAdded.AddDays(plant.WateringIntervalDays);

        return new PlantResponse(
            plant.Id,
            plant.Name,
            plant.Species,
            plant.Description,
            plant.DateAdded,
            plant.LastWatered,
            plant.WateringIntervalDays,
            plant.Location,
            plant.ImageUrl,
            nextWateringDue
        );
    }
}
