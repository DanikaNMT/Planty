namespace Planty.Application.Queries.GetSharesCreatedByUser;

using MediatR;
using Planty.Contracts.Shares;

public record GetSharesCreatedByUserQuery(Guid UserId) : IRequest<IEnumerable<ShareResponse>>;
