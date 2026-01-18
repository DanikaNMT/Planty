namespace Planty.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Planty.Application.Commands.CreatePlant;
using Planty.Application.Commands.UpdatePlant;
using Planty.Application.Commands.FertilizePlant;
using Planty.Application.Queries.GetPlantById;
using Planty.Application.Queries.GetPlants;
using Planty.Application.Queries.GetPlantWaterings;
using Planty.Application.Queries.GetPlantFertilizations;
using Planty.Application.Queries.GetPlantCareHistory;
using Planty.Application.Queries.GetPlantTodos;
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
    private readonly ILogger<PlantsController> _logger;

    public PlantsController(IMediator mediator, ILogger<PlantsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlantResponse>>> GetPlants(CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetPlants request received - Authenticated: {IsAuthenticated}, AuthType: {AuthType}",
            User.Identity?.IsAuthenticated ?? false, User.Identity?.AuthenticationType ?? "none");
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _logger.LogInformation("GetPlants - UserId from claims: {UserId}", userId ?? "null");
        
        if (User.Identity?.IsAuthenticated == true)
        {
            _logger.LogDebug("GetPlants - Claims count: {ClaimsCount}", User.Claims.Count());
            foreach (var claim in User.Claims)
            {
                _logger.LogDebug("GetPlants - Claim: {ClaimType} = {ClaimValue}", claim.Type, claim.Value);
            }
        }
        
        if (userId == null) 
        {
            _logger.LogWarning("GetPlants request unauthorized - No user ID found in claims");
            return Unauthorized();
        }
        
        var query = new GetPlantsQuery(Guid.Parse(userId));
        var result = await _mediator.Send(query, cancellationToken);
        _logger.LogInformation("GetPlants returning {PlantCount} plants for UserId: {UserId}", result.Count(), userId);
        return Ok(result);
    }

    [HttpGet("todos")]
    public async Task<ActionResult<IEnumerable<PlantTodoResponse>>> GetPlantTodos(
        [FromQuery] int hoursAhead = 24, 
        CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        var query = new GetPlantTodosQuery(Guid.Parse(userId), hoursAhead);
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
            request.SpeciesId,
            request.Description,
            request.LocationId,
            Guid.Parse(userId)
        );
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetPlant), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<PlantResponse>> UpdatePlant(Guid id, UpdatePlantRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        try
        {
            var command = new UpdatePlantCommand(
                id,
                Guid.Parse(userId),
                request.Name,
                request.SpeciesId,
                request.Description,
                request.LocationId
            );
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
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