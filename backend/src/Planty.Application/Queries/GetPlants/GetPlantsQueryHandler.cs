namespace PlantApp.Application.Queries.GetPlants;

using MediatR;
using PlantApp.Contracts.Plants;
using PlantApp.Domain.Entities;
using PlantApp.Domain.Repositories;

public class GetPlantsQueryHandler : IRequestHandler<GetPlantsQuery, IEnumerable<PlantResponse>>
{
    private readonly IPlantRepository _plantRepository;

    public GetPlantsQueryHandler(IPlantRepository plantRepository)
    {
        _plantRepository = plantRepository;
    }

    public async Task<IEnumerable<PlantResponse>> Handle(GetPlantsQuery request, CancellationToken cancellationToken)
    {
        var plants = await _plantRepository.GetAllAsync(cancellationToken);
        return plants.Select(MapToResponse);
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