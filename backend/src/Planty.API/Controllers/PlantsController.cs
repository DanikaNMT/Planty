namespace Planty.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Planty.Application.Commands.CreatePlant;
using Planty.Application.Commands.FertilizePlant;
using Planty.Application.Queries.GetPlantById;
using Planty.Application.Queries.GetPlants;
using Planty.Application.Queries.GetPlantWaterings;
using Planty.Application.Queries.GetPlantFertilizations;
using Planty.Application.Queries.GetPlantCareHistory;
using Planty.Application.Commands.WaterPlant;
using Planty.Application.Commands.UploadPlantPicture;
using Planty.Contracts.Plants;

using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

[ApiController]
[Route("api/plants")]
[Authorize]
public class PlantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlantResponse>>> GetPlants(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        var query = new GetPlantsQuery(Guid.Parse(userId));
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PlantResponse>> GetPlant(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        var query = new GetPlantByIdQuery(id, Guid.Parse(userId));
        var result = await _mediator.Send(query, cancellationToken);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<PlantResponse>> CreatePlant(CreatePlantRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        var command = new CreatePlantCommand(
            request.Name,
            request.Species,
            request.Description,
            request.WateringIntervalDays,
            request.FertilizationIntervalDays,
            request.LocationId,
            Guid.Parse(userId)
        );
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetPlant), new { id = result.Id }, result);
    }
    [HttpPost("{id:guid}/water")]
    public async Task<ActionResult<PlantResponse>> WaterPlant(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        try
        {
            var command = new WaterPlantCommand(id, Guid.Parse(userId));
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}/waterings")]
    public async Task<ActionResult<IEnumerable<WateringResponse>>> GetPlantWaterings(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        var query = new GetPlantWateringsQuery(id, Guid.Parse(userId));
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:guid}/fertilize")]
    public async Task<ActionResult<PlantResponse>> FertilizePlant(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        try
        {
            var command = new FertilizePlantCommand(id, Guid.Parse(userId));
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}/fertilizations")]
    public async Task<ActionResult<IEnumerable<FertilizationResponse>>> GetPlantFertilizations(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        var query = new GetPlantFertilizationsQuery(id, Guid.Parse(userId));
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:guid}/care-history")]
    public async Task<ActionResult<IEnumerable<CareEventResponse>>> GetPlantCareHistory(Guid id, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        var query = new GetPlantCareHistoryQuery(id, Guid.Parse(userId));
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}