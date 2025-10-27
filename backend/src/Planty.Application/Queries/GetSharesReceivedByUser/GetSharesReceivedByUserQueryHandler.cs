namespace Planty.Application.Queries.GetSharesReceivedByUser;

using MediatR;
using Planty.Contracts.Shares;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

public class GetSharesReceivedByUserQueryHandler : IRequestHandler<GetSharesReceivedByUserQuery, IEnumerable<ShareResponse>>
{
    private readonly IShareRepository _shareRepository;

    public GetSharesReceivedByUserQueryHandler(IShareRepository shareRepository)
    {
        _shareRepository = shareRepository;
    }

    public async Task<IEnumerable<ShareResponse>> Handle(GetSharesReceivedByUserQuery request, CancellationToken cancellationToken)
    {
        var shares = await _shareRepository.GetSharesReceivedByUserAsync(request.UserId, cancellationToken);
        return shares.Select(MapToResponse);
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
