namespace Planty.Application.Queries.SearchUsers;

using MediatR;
using Planty.Contracts.Shares;

public record SearchUsersQuery(string SearchTerm) : IRequest<IEnumerable<UserSearchResult>>;
