namespace Planty.Application.Queries.GetSharesReceivedByUser;

using MediatR;
using Planty.Contracts.Shares;

public record GetSharesReceivedByUserQuery(Guid UserId) : IRequest<IEnumerable<ShareResponse>>;
