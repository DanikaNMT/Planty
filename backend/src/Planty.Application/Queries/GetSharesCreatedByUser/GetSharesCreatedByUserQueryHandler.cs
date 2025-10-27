namespace Planty.Application.Queries.GetSharesCreatedByUser;

using MediatR;
using Planty.Contracts.Shares;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

public class GetSharesCreatedByUserQueryHandler : IRequestHandler<GetSharesCreatedByUserQuery, IEnumerable<ShareResponse>>
{
    private readonly IShareRepository _shareRepository;

    public GetSharesCreatedByUserQueryHandler(IShareRepository shareRepository)
    {
        _shareRepository = shareRepository;
    }

    public async Task<IEnumerable<ShareResponse>> Handle(GetSharesCreatedByUserQuery request, CancellationToken cancellationToken)
    {
        var shares = await _shareRepository.GetSharesCreatedByUserAsync(request.UserId, cancellationToken);
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
