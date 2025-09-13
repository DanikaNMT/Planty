namespace Planty.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Planty.Application.Commands.CreatePlant;
using Planty.Application.Queries.GetPlantById;
using Planty.Application.Queries.GetPlants;
using Planty.Application.Commands.WaterPlant;
using Planty.Contracts.Plants;

[ApiController]
[Route("api/plants")]
public class PlantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlantResponse>>> GetPlants(
        CancellationToken cancellationToken)
    {
        var query = new GetPlantsQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PlantResponse>> GetPlant(
        Guid id, 
        CancellationToken cancellationToken)
    {
        var query = new GetPlantByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<PlantResponse>> CreatePlant(
        CreatePlantRequest request, 
        CancellationToken cancellationToken)
    {
        var command = new CreatePlantCommand(
            request.Name,
            request.Species,
            request.Description,
            request.WateringIntervalDays,
            request.Location
        );

        var result = await _mediator.Send(command, cancellationToken);
        
        return CreatedAtAction(
            nameof(GetPlant), 
            new { id = result.Id }, 
            result);
    }
    [HttpPost("{id:guid}/water")]
    public async Task<ActionResult<PlantResponse>> WaterPlant(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var command = new WaterPlantCommand(id);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}