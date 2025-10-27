namespace Planty.Application.Commands.UpdateShare;

using MediatR;
using Planty.Contracts.Shares;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

public class UpdateShareCommandHandler : IRequestHandler<UpdateShareCommand, ShareResponse>
{
    private readonly IShareRepository _shareRepository;

    public UpdateShareCommandHandler(IShareRepository shareRepository)
    {
        _shareRepository = shareRepository;
    }

    public async Task<ShareResponse> Handle(UpdateShareCommand request, CancellationToken cancellationToken)
    {
        var share = await _shareRepository.GetByIdAsync(request.ShareId, cancellationToken);
        
        if (share == null)
        {
            throw new InvalidOperationException($"Share with ID {request.ShareId} not found.");
        }

        // Only the owner can update the share
        if (share.OwnerId != request.UserId)
        {
            throw new UnauthorizedAccessException("You don't have permission to update this share.");
        }

        share.Role = (ShareRole)request.Role;

        await _shareRepository.UpdateAsync(share, cancellationToken);
        await _shareRepository.SaveChangesAsync(cancellationToken);

        // Reload to get related data
        var updated = await _shareRepository.GetByIdAsync(share.Id, cancellationToken);
        return MapToResponse(updated!);
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
