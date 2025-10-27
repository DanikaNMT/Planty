namespace Planty.Application.Queries.SearchUsers;

using MediatR;
using Planty.Contracts.Shares;
using Planty.Domain.Repositories;

public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, IEnumerable<UserSearchResult>>
{
    private readonly IUserRepository _userRepository;

    public SearchUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserSearchResult>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.SearchUsersAsync(request.SearchTerm, 10, cancellationToken);
        return users.Select(u => new UserSearchResult(u.Id, u.UserName, u.Email));
    }
}
