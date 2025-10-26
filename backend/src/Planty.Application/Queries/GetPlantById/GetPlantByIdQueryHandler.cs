namespace Planty.Application.Queries.GetPlantById;

using MediatR;
using Planty.Contracts.Plants;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

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
        if (plant == null || plant.UserId != request.UserId)
            return null;
        return MapToResponse(plant);
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