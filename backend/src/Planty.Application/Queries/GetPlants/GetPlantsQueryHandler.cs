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
        DateTime? nextWateringDue = null;
        if (plant.WateringIntervalDays.HasValue)
        {
            nextWateringDue = plant.LastWatered?.AddDays(plant.WateringIntervalDays.Value) ?? 
                             plant.DateAdded.AddDays(plant.WateringIntervalDays.Value);
        }

        return new PlantResponse(
            plant.Id,
            plant.Name,
            plant.Species,
            plant.Description,
            plant.DateAdded,
            plant.LastWatered,
            plant.WateringIntervalDays,
            plant.Location?.Name,
            plant.ImageUrl,
            nextWateringDue
        );
    }
}