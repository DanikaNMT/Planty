namespace PlantApp.Application.Queries.GetPlantById;

using MediatR;
using PlantApp.Contracts.Plants;
using PlantApp.Domain.Entities;
using PlantApp.Domain.Repositories;

public class GetPlantByIdQueryHandler : IRequestHandler<GetPlantByIdQuery, PlantResponse?>
{
    private readonly IPlantRepository _plantRepository;

    public GetPlantByIdQueryHandler(IPlantRepository plantRepository)
    {
        _plantRepository = plantRepository;
    }

    public async Task<PlantResponse?> Handle(GetPlantByIdQuery request, CancellationToken cancellationToken)
    {
        var plant = await _plantRepository.GetByIdAsync(request.Id, cancellationToken);
        return plant == null ? null : MapToResponse(plant);
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