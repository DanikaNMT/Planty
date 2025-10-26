namespace Planty.Application.Commands.DeleteLocation;

using MediatR;
using Planty.Domain.Repositories;

public class DeleteLocationCommandHandler : IRequestHandler<DeleteLocationCommand, bool>
{
    private readonly ILocationRepository _locationRepository;

    public DeleteLocationCommandHandler(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<bool> Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken);
        
        if (location == null || location.UserId != request.UserId)
        {
            return false;
        }

        // Don't allow deleting default location
        if (location.IsDefault)
        {
            return false;
        }

        // Plants in this location will have their LocationId set to null (SetNull cascade)
        await _locationRepository.DeleteAsync(request.LocationId, cancellationToken);
        
        return true;
    }
}
