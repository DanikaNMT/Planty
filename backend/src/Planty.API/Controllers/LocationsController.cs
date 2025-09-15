namespace Planty.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planty.Application.Commands.CreateLocation;
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
        return CreatedAtAction(nameof(GetLocations), new { id = result.Id }, result);
    }
}