namespace Planty.Application.Queries.GetPlantById;

using MediatR;
using Planty.Application.Common;
using Planty.Contracts.Plants;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

public class GetPlantByIdQueryHandler : IRequestHandler<GetPlantByIdQuery, PlantResponse?>
{
    private readonly IPlantRepository _plantRepository;
    private readonly IShareRepository _shareRepository;

    public GetPlantByIdQueryHandler(IPlantRepository plantRepository, IShareRepository shareRepository)
    {
        _plantRepository = plantRepository;
        _shareRepository = shareRepository;
    }

    public async Task<PlantResponse?> Handle(GetPlantByIdQuery request, CancellationToken cancellationToken)
    {
        var plant = await _plantRepository.GetByIdAsync(request.Id, cancellationToken);
        if (plant == null)
            return null;
            
        // Check if user owns the plant
        if (plant.UserId == request.UserId)
        {
            return PlantMapper.MapToResponse(plant, isShared: false);
        }
        
        // Check if plant is shared with user
        var role = await _shareRepository.GetUserRoleForPlantAsync(request.Id, request.UserId, cancellationToken);
        if (!role.HasValue)
            return null; // User has no access
            
        var roleDto = (Contracts.Shares.ShareRoleDto)role.Value;
        return PlantMapper.MapToResponse(
            plant, 
            isShared: true, 
            userRole: roleDto,
            ownerId: plant.UserId,
            ownerName: plant.User?.UserName
        );
    }
}
