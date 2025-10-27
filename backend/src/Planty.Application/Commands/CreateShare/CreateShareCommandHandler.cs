namespace Planty.Application.Commands.CreateShare;

using MediatR;
using Planty.Application.Interfaces;
using Planty.Contracts.Shares;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

public class CreateShareCommandHandler : IRequestHandler<CreateShareCommand, ShareResponse>
{
    private readonly IShareRepository _shareRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPlantRepository _plantRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IPermissionService _permissionService;

    public CreateShareCommandHandler(
        IShareRepository shareRepository,
        IUserRepository userRepository,
        IPlantRepository plantRepository,
        ILocationRepository locationRepository,
        IPermissionService permissionService)
    {
        _shareRepository = shareRepository;
        _userRepository = userRepository;
        _plantRepository = plantRepository;
        _locationRepository = locationRepository;
        _permissionService = permissionService;
    }

    public async Task<ShareResponse> Handle(CreateShareCommand request, CancellationToken cancellationToken)
    {
        // Find the user to share with
        var sharedWithUser = await _userRepository.GetByEmailAsync(request.SharedWithUserEmail, cancellationToken);
        if (sharedWithUser == null)
        {
            throw new InvalidOperationException($"User with email {request.SharedWithUserEmail} not found.");
        }

        // Can't share with yourself
        if (sharedWithUser.Id == request.OwnerId)
        {
            throw new InvalidOperationException("You cannot share with yourself.");
        }

        Share share;

        if (request.ShareType == ShareTypeDto.Plant)
        {
            // Verify plant exists and user has permission to share it
            var plant = await _plantRepository.GetByIdAsync(request.PlantId!.Value, cancellationToken);
            if (plant == null)
            {
                throw new InvalidOperationException($"Plant with ID {request.PlantId} not found.");
            }

            if (!await _permissionService.CanUserSharePlantAsync(plant.Id, request.OwnerId, cancellationToken))
            {
                throw new UnauthorizedAccessException("You don't have permission to share this plant.");
            }

            // Check if already shared with this user
            var existingShare = await _shareRepository.GetShareForPlantAndUserAsync(plant.Id, sharedWithUser.Id, cancellationToken);
            if (existingShare != null)
            {
                throw new InvalidOperationException($"Plant is already shared with {sharedWithUser.Email}.");
            }

            share = new Share
            {
                OwnerId = request.OwnerId,
                SharedWithUserId = sharedWithUser.Id,
                ShareType = ShareType.Plant,
                PlantId = plant.Id,
                Role = (ShareRole)request.Role
            };
        }
        else if (request.ShareType == ShareTypeDto.Location)
        {
            // Verify location exists and user has permission to share it
            var location = await _locationRepository.GetByIdAsync(request.LocationId!.Value, cancellationToken);
            if (location == null)
            {
                throw new InvalidOperationException($"Location with ID {request.LocationId} not found.");
            }

            if (!await _permissionService.CanUserShareLocationAsync(location.Id, request.OwnerId, cancellationToken))
            {
                throw new UnauthorizedAccessException("You don't have permission to share this location.");
            }

            // Check if already shared with this user
            var existingShare = await _shareRepository.GetShareForLocationAndUserAsync(location.Id, sharedWithUser.Id, cancellationToken);
            if (existingShare != null)
            {
                throw new InvalidOperationException($"Location is already shared with {sharedWithUser.Email}.");
            }

            share = new Share
            {
                OwnerId = request.OwnerId,
                SharedWithUserId = sharedWithUser.Id,
                ShareType = ShareType.Location,
                LocationId = location.Id,
                Role = (ShareRole)request.Role
            };
        }
        else // Collection
        {
            // Check if collection is already shared with this user
            var existingShare = await _shareRepository.GetCollectionShareAsync(request.OwnerId, sharedWithUser.Id, cancellationToken);
            if (existingShare != null)
            {
                throw new InvalidOperationException($"Your collection is already shared with {sharedWithUser.Email}.");
            }

            share = new Share
            {
                OwnerId = request.OwnerId,
                SharedWithUserId = sharedWithUser.Id,
                ShareType = ShareType.Collection,
                PlantId = null,
                LocationId = null,
                Role = (ShareRole)request.Role
            };
        }

        await _shareRepository.AddAsync(share, cancellationToken);
        await _shareRepository.SaveChangesAsync(cancellationToken);

        // Reload to get related data
        var created = await _shareRepository.GetByIdAsync(share.Id, cancellationToken);
        return MapToResponse(created!);
    }

    private static ShareResponse MapToResponse(Share share)
    {
        return new ShareResponse(
            share.Id,
            (ShareTypeDto)share.ShareType,
            new UserInfo(share.OwnerId, share.Owner.UserName, share.Owner.Email),
            new UserInfo(share.SharedWithUserId, share.SharedWithUser.UserName, share.SharedWithUser.Email),
            share.PlantId,
            share.Plant?.Name,
            share.LocationId,
            share.Location?.Name,
            (ShareRoleDto)share.Role,
            share.CreatedAt
        );
    }
}
