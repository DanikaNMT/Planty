namespace Planty.Application.Queries.GetPlants;

using MediatR;
using Planty.Application.Common;
using Planty.Contracts.Plants;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

public class GetPlantsQueryHandler : IRequestHandler<GetPlantsQuery, IEnumerable<PlantResponse>>
{
    private readonly IPlantRepository _plantRepository;
    private readonly IShareRepository _shareRepository;

    public GetPlantsQueryHandler(IPlantRepository plantRepository, IShareRepository shareRepository)
    {
        _plantRepository = plantRepository;
        _shareRepository = shareRepository;
    }

    public async Task<IEnumerable<PlantResponse>> Handle(GetPlantsQuery request, CancellationToken cancellationToken)
    {
        // Get plants owned by user
        var ownedPlants = await _plantRepository.GetAllByUserAsync(request.UserId, cancellationToken);
        
        // Get shared plant IDs
        var sharedPlantIds = await _shareRepository.GetSharedPlantIdsForUserAsync(request.UserId, cancellationToken);
        
        // Build result list
        var result = new List<PlantResponse>();
        
        // Add owned plants (not shared)
        foreach (var plant in ownedPlants)
        {
            result.Add(PlantMapper.MapToResponse(plant, isShared: false));
        }
        
        // Add shared plants with role information
        foreach (var plantId in sharedPlantIds)
        {
            // Skip if already added as owned plant
            if (ownedPlants.Any(p => p.Id == plantId))
                continue;
                
            var plant = await _plantRepository.GetByIdAsync(plantId, cancellationToken);
            if (plant != null)
            {
                // Get user's role for this plant
                var role = await _shareRepository.GetUserRoleForPlantAsync(plantId, request.UserId, cancellationToken);
                var roleDto = role.HasValue ? (Contracts.Shares.ShareRoleDto)role.Value : (Contracts.Shares.ShareRoleDto?)null;
                
                result.Add(PlantMapper.MapToResponse(
                    plant, 
                    isShared: true, 
                    userRole: roleDto,
                    ownerId: plant.UserId,
                    ownerName: plant.User?.UserName
                ));
            }
        }
        
        return result;
    }
}