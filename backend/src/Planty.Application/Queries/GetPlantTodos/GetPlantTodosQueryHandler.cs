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
            // Check for watering todos
            if (plant.WateringIntervalDays.HasValue)
            {
                var lastWatered = plant.Waterings.OrderByDescending(w => w.WateredAt).FirstOrDefault()?.WateredAt;
                var nextWateringDue = lastWatered.HasValue 
                    ? lastWatered.Value.AddDays(plant.WateringIntervalDays.Value)
                    : plant.DateAdded.AddDays(plant.WateringIntervalDays.Value);

                if (nextWateringDue <= cutoffDate)
                {
                    var latestPicture = plant.Pictures.OrderByDescending(p => p.TakenAt).FirstOrDefault();
                    todos.Add(new PlantTodoResponse(
                        plant.Id,
                        plant.Name,
                        plant.Species,
                        "Water",
                        nextWateringDue,
                        latestPicture != null ? $"/api/plants/pictures/{latestPicture.Id}" : null
                    ));
                }
            }

            // Check for fertilization todos
            if (plant.FertilizationIntervalDays.HasValue)
            {
                var lastFertilized = plant.Fertilizations.OrderByDescending(f => f.FertilizedAt).FirstOrDefault()?.FertilizedAt;
                var nextFertilizationDue = lastFertilized.HasValue
                    ? lastFertilized.Value.AddDays(plant.FertilizationIntervalDays.Value)
                    : plant.DateAdded.AddDays(plant.FertilizationIntervalDays.Value);

                if (nextFertilizationDue <= cutoffDate)
                {
                    var latestPicture = plant.Pictures.OrderByDescending(p => p.TakenAt).FirstOrDefault();
                    todos.Add(new PlantTodoResponse(
                        plant.Id,
                        plant.Name,
                        plant.Species,
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
