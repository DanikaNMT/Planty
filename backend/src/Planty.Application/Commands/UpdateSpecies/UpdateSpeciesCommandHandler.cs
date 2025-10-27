namespace Planty.Application.Commands.UpdateSpecies;

using MediatR;
using Planty.Application.Interfaces;
using Planty.Contracts.Species;

public class UpdateSpeciesCommandHandler : IRequestHandler<UpdateSpeciesCommand, SpeciesResponse>
{
    private readonly ISpeciesRepository _speciesRepository;

    public UpdateSpeciesCommandHandler(ISpeciesRepository speciesRepository)
    {
        _speciesRepository = speciesRepository;
    }

    public async Task<SpeciesResponse> Handle(UpdateSpeciesCommand request, CancellationToken cancellationToken)
    {
        var species = await _speciesRepository.GetByIdAsync(request.SpeciesId, cancellationToken);
        
        if (species == null)
        {
            throw new InvalidOperationException($"Species with ID {request.SpeciesId} not found.");
        }

        if (species.UserId != request.UserId)
        {
            throw new UnauthorizedAccessException("You don't have permission to update this species.");
        }

        species.Name = request.Name;
        species.Description = request.Description;
        species.WateringIntervalDays = request.WateringIntervalDays;
        species.FertilizationIntervalDays = request.FertilizationIntervalDays;

        await _speciesRepository.UpdateAsync(species, cancellationToken);

        // Reload to get plant count
        var updatedSpecies = await _speciesRepository.GetByIdAsync(species.Id, cancellationToken);
        
        return new SpeciesResponse(
            updatedSpecies!.Id,
            updatedSpecies.Name,
            updatedSpecies.Description,
            updatedSpecies.WateringIntervalDays,
            updatedSpecies.FertilizationIntervalDays,
            updatedSpecies.Plants.Count,
            false, // Not shared - user owns it
            null,  // No owner ID for owned species
            null   // No owner name for owned species
        );
    }
}
