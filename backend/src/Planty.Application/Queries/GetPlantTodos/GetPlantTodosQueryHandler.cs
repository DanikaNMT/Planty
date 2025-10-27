namespace Planty.Application.Queries.GetPlantTodos;

using MediatR;
using Planty.Contracts.Plants;
using Planty.Domain.Repositories;

public class GetPlantTodosQueryHandler : IRequestHandler<GetPlantTodosQuery, IEnumerable<PlantTodoResponse>>
{
    private readonly IPlantRepository _plantRepository;
    private readonly IShareRepository _shareRepository;

    public GetPlantTodosQueryHandler(IPlantRepository plantRepository, IShareRepository shareRepository)
    {
        _plantRepository = plantRepository;
        _shareRepository = shareRepository;
    }

    public async Task<IEnumerable<PlantTodoResponse>> Handle(GetPlantTodosQuery request, CancellationToken cancellationToken)
    {
        // Get owned plants
        var ownedPlants = await _plantRepository.GetAllByUserAsync(request.UserId, cancellationToken);
        
        // Get shared plant IDs
        var sharedPlantIds = await _shareRepository.GetSharedPlantIdsForUserAsync(request.UserId, cancellationToken);
        
        // Get shared plants
        var sharedPlants = new List<Domain.Entities.Plant>();
        foreach (var plantId in sharedPlantIds)
        {
            // Skip if already in owned plants
            if (ownedPlants.Any(p => p.Id == plantId))
                continue;
                
            var plant = await _plantRepository.GetByIdAsync(plantId, cancellationToken);
            if (plant != null)
            {
                sharedPlants.Add(plant);
            }
        }
        
        // Combine all plants
        var allPlants = ownedPlants.Concat(sharedPlants);
        
        var cutoffDate = DateTime.UtcNow.AddHours(request.HoursAhead);
        var todos = new List<PlantTodoResponse>();

        foreach (var plant in allPlants)
        {
            // Skip plants without species (no care intervals defined)
            if (plant.Species == null)
                continue;

            // Determine if plant is shared and get user's role
            bool isShared = plant.UserId != request.UserId;
            var userRole = isShared 
                ? await _shareRepository.GetUserRoleForPlantAsync(plant.Id, request.UserId, cancellationToken)
                : null;
            var roleDto = userRole.HasValue ? (Contracts.Shares.ShareRoleDto?)userRole.Value : null;

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
                        latestPicture != null ? $"/api/plants/pictures/{latestPicture.Id}" : null,
                        isShared,
                        roleDto,
                        isShared ? plant.UserId : null,
                        isShared ? plant.User?.UserName : null
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
                        latestPicture != null ? $"/api/plants/pictures/{latestPicture.Id}" : null,
                        isShared,
                        roleDto,
                        isShared ? plant.UserId : null,
                        isShared ? plant.User?.UserName : null
                    ));
                }
            }
        }

        // Sort by due date (most urgent first)
        return todos.OrderBy(t => t.DueDate).ToList();
    }
}
