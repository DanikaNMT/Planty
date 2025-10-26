namespace Planty.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planty.Application.Commands.CreateSpecies;
using Planty.Application.Commands.UpdateSpecies;
using Planty.Application.Commands.DeleteSpecies;
using Planty.Application.Queries.GetSpecies;
using Planty.Application.Queries.GetSpeciesById;
using Planty.Contracts.Species;
using System.Security.Claims;

[ApiController]
[Route("api/species")]
[Authorize]
public class SpeciesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SpeciesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SpeciesResponse>>> GetSpecies(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        
        var query = new GetSpeciesQuery(Guid.Parse(userId));
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SpeciesResponse>> GetSpeciesById(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        
        var query = new GetSpeciesByIdQuery(id, Guid.Parse(userId));
        var result = await _mediator.Send(query, cancellationToken);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<SpeciesResponse>> CreateSpecies(CreateSpeciesRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var command = new CreateSpeciesCommand(
            request.Name,
            request.Description,
            request.WateringIntervalDays,
            request.FertilizationIntervalDays,
            Guid.Parse(userId)
        );

        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetSpeciesById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SpeciesResponse>> UpdateSpecies(Guid id, UpdateSpeciesRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        try
        {
            var command = new UpdateSpeciesCommand(
                id,
                Guid.Parse(userId),
                request.Name,
                request.Description,
                request.WateringIntervalDays,
                request.FertilizationIntervalDays
            );

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteSpecies(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        try
        {
            var command = new DeleteSpeciesCommand(id, Guid.Parse(userId));
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
