namespace Planty.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planty.Application.Commands.CreateLocation;
using Planty.Application.Commands.DeleteLocation;
using Planty.Application.Commands.UpdateLocation;
using Planty.Application.Queries.GetLocationById;
using Planty.Application.Queries.GetLocations;
using Planty.Contracts.Locations;
using System.Security.Claims;

[ApiController]
[Route("api/locations")]
[Authorize]
public class LocationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LocationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LocationResponse>>> GetLocations(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        
        var query = new GetLocationsQuery(Guid.Parse(userId));
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LocationDetailResponse>> GetLocationById(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        
        var query = new GetLocationByIdQuery(id, Guid.Parse(userId));
        var result = await _mediator.Send(query, cancellationToken);
        
        if (result == null) return NotFound();
        
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<LocationResponse>> CreateLocation(CreateLocationRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var command = new CreateLocationCommand(
            request.Name,
            request.Description,
            Guid.Parse(userId)
        );

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetLocationById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<LocationResponse>> UpdateLocation(Guid id, UpdateLocationRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var command = new UpdateLocationCommand(
            id,
            request.Name,
            request.Description,
            Guid.Parse(userId)
        );

        var result = await _mediator.Send(command, cancellationToken);
        
        if (result == null) return NotFound();
        
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteLocation(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var command = new DeleteLocationCommand(id, Guid.Parse(userId));
        var result = await _mediator.Send(command, cancellationToken);
        
        if (!result) return NotFound();
        
        return NoContent();
    }
}