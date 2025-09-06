namespace PlantApp.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using PlantApp.Application.Commands.CreatePlant;
using PlantApp.Application.Queries.GetPlantById;
using PlantApp.Application.Queries.GetPlants;
using PlantApp.Contracts.Plants;

[ApiController]
[Route("api/[controller]")]
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
}