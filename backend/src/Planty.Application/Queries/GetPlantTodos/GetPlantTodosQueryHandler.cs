namespace Planty.Application.Queries.GetPlantTodos;

using MediatR;
using Planty.Contracts.Plants;
using Planty.Domain.Repositories;

public class GetPlantTodosQueryHandler : IRequestHandler<GetPlantTodosQuery, IEnumerable<PlantTodoResponse>>
{
    private readonly IPlantRepository _plantRepository;

    public GetPlantTodosQueryHandler(IPlantRepository plantRepository)
    {
        _plantRepository = plantRepository;
    }

    public async Task<IEnumerable<PlantTodoResponse>> Handle(GetPlantTodosQuery request, CancellationToken cancellationToken)
    {
        var plants = await _plantRepository.GetAllByUserAsync(request.UserId, cancellationToken);
        var cutoffDate = DateTime.UtcNow.AddHours(request.HoursAhead);
        var todos = new List<PlantTodoResponse>();

        foreach (var plant in plants)
        {
            // Skip plants without species (no care intervals defined)
            if (plant.Species == null)
                continue;

            // Check for watering todos
            if (plant.Species.WateringIntervalDays.HasValue)
            {
                var lastWatered = plant.Waterings.OrderByDescending(w => w.WateredAt).FirstOrDefault()?.WateredAt;
                var nextWateringDue = lastWatered.HasValue 
                    ? lastWatered.Value.AddDays(plant.Species.WateringIntervalDays.Value)
                    : plant.DateAdded.AddDays(plant.Species.WateringIntervalDays.Value);

                if (nextWateringDue <= cutoffDate)
                {
                    var latestPicture = plant.Pictures.OrderByDescending(p => p.TakenAt).FirstOrDefault();
                    todos.Add(new PlantTodoResponse(
                        plant.Id,
                        plant.Name,
                        plant.Species.Name,
                        "Water",
                        nextWateringDue,
                        latestPicture != null ? $"/api/plants/pictures/{latestPicture.Id}" : null
                    ));
                }
            }

            // Check for fertilization todos
            if (plant.Species.FertilizationIntervalDays.HasValue)
            {
                var lastFertilized = plant.Fertilizations.OrderByDescending(f => f.FertilizedAt).FirstOrDefault()?.FertilizedAt;
                var nextFertilizationDue = lastFertilized.HasValue
                    ? lastFertilized.Value.AddDays(plant.Species.FertilizationIntervalDays.Value)
                    : plant.DateAdded.AddDays(plant.Species.FertilizationIntervalDays.Value);

                if (nextFertilizationDue <= cutoffDate)
                {
                    var latestPicture = plant.Pictures.OrderByDescending(p => p.TakenAt).FirstOrDefault();
                    todos.Add(new PlantTodoResponse(
                        plant.Id,
                        plant.Name,
                        plant.Species.Name,
                        "Fertilize",
                        nextFertilizationDue,
                        latestPicture != null ? $"/api/plants/pictures/{latestPicture.Id}" : null
                    ));
                }
            }
        }

        // Sort by due date (most urgent first)
        return todos.OrderBy(t => t.DueDate).ToList();
    }
}
