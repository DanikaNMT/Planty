namespace Planty.Application.Queries.GetSpecies;

using MediatR;
using Planty.Application.Interfaces;
using Planty.Contracts.Species;

public class GetSpeciesQueryHandler : IRequestHandler<GetSpeciesQuery, IEnumerable<SpeciesResponse>>
{
    private readonly ISpeciesRepository _speciesRepository;

    public GetSpeciesQueryHandler(ISpeciesRepository speciesRepository)
    {
        _speciesRepository = speciesRepository;
    }

    public async Task<IEnumerable<SpeciesResponse>> Handle(GetSpeciesQuery request, CancellationToken cancellationToken)
    {
        var species = await _speciesRepository.GetAllByUserAsync(request.UserId, cancellationToken);
        
        return species.Select(s => new SpeciesResponse(
            s.Id,
            s.Name,
            s.Description,
            s.WateringIntervalDays,
            s.FertilizationIntervalDays,
            s.Plants.Count
        )).ToList();
    }
}
