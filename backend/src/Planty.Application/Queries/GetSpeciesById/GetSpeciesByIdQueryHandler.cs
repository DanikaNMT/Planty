namespace Planty.Application.Queries.GetSpeciesById;

using MediatR;
using Planty.Application.Interfaces;
using Planty.Contracts.Species;
using Planty.Domain.Repositories;

public class GetSpeciesByIdQueryHandler : IRequestHandler<GetSpeciesByIdQuery, SpeciesResponse?>
{
    private readonly ISpeciesRepository _speciesRepository;
    private readonly IShareRepository _shareRepository;

    public GetSpeciesByIdQueryHandler(ISpeciesRepository speciesRepository, IShareRepository shareRepository)
    {
        _speciesRepository = speciesRepository;
        _shareRepository = shareRepository;
    }

    public async Task<SpeciesResponse?> Handle(GetSpeciesByIdQuery request, CancellationToken cancellationToken)
    {
        var species = await _speciesRepository.GetByIdAsync(request.SpeciesId, cancellationToken);
        
        if (species == null)
        {
            return null;
        }

        bool isOwner = species.UserId == request.UserId;
        bool hasAccess = isOwner;

        // If not owner, check if user has access via collection share
        if (!hasAccess)
        {
            var collectionOwnerIds = await _shareRepository.GetOwnerIdsWithCollectionAccessAsync(request.UserId, cancellationToken);
            hasAccess = collectionOwnerIds.Contains(species.UserId);
        }

        // If still no access, check if user has access to any plant with this species
        if (!hasAccess)
        {
            var sharedPlantIds = await _shareRepository.GetSharedPlantIdsForUserAsync(request.UserId, cancellationToken);
            hasAccess = species.Plants.Any(p => sharedPlantIds.Contains(p.Id));
        }

        if (!hasAccess)
        {
            return null;
        }

        return new SpeciesResponse(
            species.Id,
            species.Name,
            species.Description,
            species.WateringIntervalDays,
            species.FertilizationIntervalDays,
            species.Plants.Count,
            !isOwner,
            !isOwner ? species.UserId : null,
            !isOwner ? species.User?.UserName : null
        );
    }
}
