namespace Planty.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planty.Application.Commands.CreateShare;
using Planty.Application.Commands.DeleteShare;
using Planty.Application.Commands.UpdateShare;
using Planty.Application.Queries.GetSharesCreatedByUser;
using Planty.Application.Queries.GetSharesReceivedByUser;
using Planty.Contracts.Shares;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SharesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SharesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new share (share a plant or location with another user)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ShareResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateShare([FromBody] CreateShareRequest request, CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdString == null) return Unauthorized();
        var userId = Guid.Parse(userIdString);
        
        try
        {
            var command = new CreateShareCommand(
                userId,
                request.ShareType,
                request.PlantId,
                request.LocationId,
                request.SharedWithUserEmail,
                request.Role
            );

            var result = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetSharesCreated), new { }, result);
        }
        catch (InvalidOperationException ex)
        {
            // Return 409 Conflict for duplicate shares or validation errors
            return Conflict(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            // Return 403 Forbidden for permission issues
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get all shares created by the current user (things I've shared with others)
    /// </summary>
    [HttpGet("created")]
    [ProducesResponseType(typeof(IEnumerable<ShareResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSharesCreated(CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdString == null) return Unauthorized();
        var userId = Guid.Parse(userIdString);
        
        var query = new GetSharesCreatedByUserQuery(userId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get all shares received by the current user (things others have shared with me)
    /// </summary>
    [HttpGet("received")]
    [ProducesResponseType(typeof(IEnumerable<ShareResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetSharesReceived(CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdString == null) return Unauthorized();
        var userId = Guid.Parse(userIdString);
        
        var query = new GetSharesReceivedByUserQuery(userId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Update a share (change the role)
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ShareResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateShare(Guid id, [FromBody] UpdateShareRequest request, CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdString == null) return Unauthorized();
        var userId = Guid.Parse(userIdString);
        
        var command = new UpdateShareCommand(id, userId, request.Role);
        var result = await _mediator.Send(command, cancellationToken);
        
        if (result == null)
            return NotFound();
        
        return Ok(result);
    }

    /// <summary>
    /// Delete a share (revoke access)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteShare(Guid id, CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdString == null) return Unauthorized();
        var userId = Guid.Parse(userIdString);
        
        var command = new DeleteShareCommand(id, userId);
        await _mediator.Send(command, cancellationToken);
        
        return NoContent();
    }
}
