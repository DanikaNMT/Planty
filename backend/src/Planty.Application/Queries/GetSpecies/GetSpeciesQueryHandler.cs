namespace Planty.Application.Queries.GetSpecies;

using MediatR;
using Planty.Application.Interfaces;
using Planty.Contracts.Species;
using Planty.Domain.Repositories;

public class GetSpeciesQueryHandler : IRequestHandler<GetSpeciesQuery, IEnumerable<SpeciesResponse>>
{
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IShareRepository _shareRepository;

    public GetSpeciesQueryHandler(ISpeciesRepository speciesRepository, IShareRepository shareRepository)
    {
        _speciesRepository = speciesRepository;
        _shareRepository = shareRepository;
    }

    public async Task<IEnumerable<SpeciesResponse>> Handle(GetSpeciesQuery request, CancellationToken cancellationToken)
    {
        // Get species owned by user
        var ownedSpecies = await _speciesRepository.GetAllByUserAsync(request.UserId, cancellationToken);
        
        // Get owner IDs of users who have shared their collection with this user
        var sharedCollectionOwnerIds = await _shareRepository.GetOwnerIdsWithCollectionAccessAsync(request.UserId, cancellationToken);
        
        // Get species from users who shared their collection
        var sharedSpecies = new List<Domain.Entities.Species>();
        foreach (var ownerId in sharedCollectionOwnerIds)
        {
            var ownerSpecies = await _speciesRepository.GetAllByUserAsync(ownerId, cancellationToken);
            sharedSpecies.AddRange(ownerSpecies);
        }
        
        // Combine and deduplicate by ID
        var allSpecies = ownedSpecies.Concat(sharedSpecies).DistinctBy(s => s.Id);
        
        return allSpecies.Select(s => new SpeciesResponse(
            s.Id,
            s.Name,
            s.Description,
            s.WateringIntervalDays,
            s.FertilizationIntervalDays,
            s.Plants.Count
        )).ToList();
    }
}
