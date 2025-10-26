namespace Planty.Application.Queries.GetSpeciesById;

using MediatR;
using Planty.Application.Interfaces;
using Planty.Contracts.Species;

public class GetSpeciesByIdQueryHandler : IRequestHandler<GetSpeciesByIdQuery, SpeciesResponse?>
{
    private readonly ISpeciesRepository _speciesRepository;

    public GetSpeciesByIdQueryHandler(ISpeciesRepository speciesRepository)
    {
        _speciesRepository = speciesRepository;
    }

    public async Task<SpeciesResponse?> Handle(GetSpeciesByIdQuery request, CancellationToken cancellationToken)
    {
        var species = await _speciesRepository.GetByIdAsync(request.SpeciesId, cancellationToken);
        
        if (species == null || species.UserId != request.UserId)
        {
            return null;
        }

        return new SpeciesResponse(
            species.Id,
            species.Name,
            species.Description,
            species.WateringIntervalDays,
            species.FertilizationIntervalDays,
            species.Plants.Count
        );
    }
}
