namespace Planty.Application.Commands.CreateSpecies;

using MediatR;
using Planty.Application.Interfaces;
using Planty.Contracts.Species;
using Planty.Domain.Entities;

public class CreateSpeciesCommandHandler : IRequestHandler<CreateSpeciesCommand, SpeciesResponse>
{
    private readonly ISpeciesRepository _speciesRepository;

    public CreateSpeciesCommandHandler(ISpeciesRepository speciesRepository)
    {
        _speciesRepository = speciesRepository;
    }

    public async Task<SpeciesResponse> Handle(CreateSpeciesCommand request, CancellationToken cancellationToken)
    {
        var species = new Species
        {
            Name = request.Name,
            Description = request.Description,
            WateringIntervalDays = request.WateringIntervalDays,
            FertilizationIntervalDays = request.FertilizationIntervalDays,
            UserId = request.UserId
        };

        var createdSpecies = await _speciesRepository.AddAsync(species, cancellationToken);

        return new SpeciesResponse(
            createdSpecies.Id,
            createdSpecies.Name,
            createdSpecies.Description,
            createdSpecies.WateringIntervalDays,
            createdSpecies.FertilizationIntervalDays,
            0, // No plants yet
            false, // Not shared - user just created it
            null, // No owner ID for owned species
            null  // No owner name for owned species
        );
    }
}
