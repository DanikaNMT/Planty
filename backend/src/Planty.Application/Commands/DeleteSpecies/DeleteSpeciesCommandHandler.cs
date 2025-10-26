namespace Planty.Application.Commands.DeleteSpecies;

using MediatR;
using Planty.Application.Interfaces;

public class DeleteSpeciesCommandHandler : IRequestHandler<DeleteSpeciesCommand, Unit>
{
    private readonly ISpeciesRepository _speciesRepository;

    public DeleteSpeciesCommandHandler(ISpeciesRepository speciesRepository)
    {
        _speciesRepository = speciesRepository;
    }

    public async Task<Unit> Handle(DeleteSpeciesCommand request, CancellationToken cancellationToken)
    {
        var species = await _speciesRepository.GetByIdAsync(request.SpeciesId, cancellationToken);
        
        if (species == null)
        {
            throw new InvalidOperationException($"Species with ID {request.SpeciesId} not found.");
        }

        if (species.UserId != request.UserId)
        {
            throw new UnauthorizedAccessException("You don't have permission to delete this species.");
        }

        await _speciesRepository.DeleteAsync(species, cancellationToken);

        return Unit.Value;
    }
}
