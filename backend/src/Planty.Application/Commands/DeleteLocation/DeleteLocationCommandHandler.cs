namespace Planty.Application.Commands.DeleteLocation;

using MediatR;
using Planty.Application.Interfaces;
using Planty.Domain.Repositories;

public class DeleteLocationCommandHandler : IRequestHandler<DeleteLocationCommand, bool>
{
    private readonly ILocationRepository _locationRepository;
    private readonly IPermissionService _permissionService;

    public DeleteLocationCommandHandler(ILocationRepository locationRepository, IPermissionService permissionService)
    {
        _locationRepository = locationRepository;
        _permissionService = permissionService;
    }

    public async Task<bool> Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken);
        
        if (location == null)
        {
            return false;
        }

        // Check if user has permission to delete this location (only Owner can delete)
        if (!await _permissionService.CanUserDeleteLocationAsync(request.LocationId, request.UserId, cancellationToken))
        {
            throw new UnauthorizedAccessException("You don't have permission to delete this location.");
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
