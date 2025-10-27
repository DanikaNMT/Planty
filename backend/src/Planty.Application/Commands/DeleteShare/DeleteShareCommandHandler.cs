namespace Planty.Application.Commands.DeleteShare;

using MediatR;
using Planty.Domain.Repositories;

public class DeleteShareCommandHandler : IRequestHandler<DeleteShareCommand, Unit>
{
    private readonly IShareRepository _shareRepository;

    public DeleteShareCommandHandler(IShareRepository shareRepository)
    {
        _shareRepository = shareRepository;
    }

    public async Task<Unit> Handle(DeleteShareCommand request, CancellationToken cancellationToken)
    {
        var share = await _shareRepository.GetByIdAsync(request.ShareId, cancellationToken);
        
        if (share == null)
        {
            throw new InvalidOperationException($"Share with ID {request.ShareId} not found.");
        }

        // Only the owner can delete the share
        if (share.OwnerId != request.UserId)
        {
            throw new UnauthorizedAccessException("You don't have permission to delete this share.");
        }

        await _shareRepository.DeleteAsync(share.Id, cancellationToken);
        await _shareRepository.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
