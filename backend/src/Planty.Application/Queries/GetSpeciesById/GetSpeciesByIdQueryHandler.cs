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

        // Check if user owns the species
        if (species.UserId == request.UserId)
        {
            return new SpeciesResponse(
                species.Id,
                species.Name,
                species.Description,
                species.WateringIntervalDays,
                species.FertilizationIntervalDays,
                species.Plants.Count
            );
        }

        // Check if user has access via collection share
        var collectionOwnerIds = await _shareRepository.GetOwnerIdsWithCollectionAccessAsync(request.UserId, cancellationToken);
        if (collectionOwnerIds.Contains(species.UserId))
        {
            return new SpeciesResponse(
                species.Id,
                species.Name,
                species.Description,
                species.WateringIntervalDays,
                species.FertilizationIntervalDays,
                species.Plants.Count
            );
        }

        // No access
        return null;
    }
}
