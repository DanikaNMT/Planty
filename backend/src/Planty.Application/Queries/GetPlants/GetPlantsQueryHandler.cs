namespace Planty.Application.Queries.GetPlants;

using MediatR;
using Planty.Contracts.Plants;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

public class GetPlantsQueryHandler : IRequestHandler<GetPlantsQuery, IEnumerable<PlantResponse>>
{
    private readonly IPlantRepository _plantRepository;

    public GetPlantsQueryHandler(IPlantRepository plantRepository)
    {
        _plantRepository = plantRepository;
    }

    public async Task<IEnumerable<PlantResponse>> Handle(GetPlantsQuery request, CancellationToken cancellationToken)
    {
        var plants = await _plantRepository.GetAllByUserAsync(request.UserId, cancellationToken);
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