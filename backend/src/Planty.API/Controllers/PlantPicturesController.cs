namespace Planty.API.Controllers;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planty.Application.Commands.UploadPlantPicture;
using Planty.Application.Services;
using Planty.Contracts.Plants;
using Planty.Domain.Repositories;
using System.Security.Claims;

[ApiController]
[Route("api/plants")]
[Authorize]
public class PlantPicturesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IPlantPictureRepository _pictureRepository;
    private readonly IFileStorageService _fileStorage;

    public PlantPicturesController(IMediator mediator, IPlantPictureRepository pictureRepository, IFileStorageService fileStorage)
    {
        _mediator = mediator;
        _pictureRepository = pictureRepository;
        _fileStorage = fileStorage;
    }

    [HttpPost("{id:guid}/pictures")]
    public async Task<ActionResult<PlantPictureResponse>> UploadPlantPicture(Guid id, [FromForm] IFormFile file, [FromForm] string? notes, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();
        
        if (file == null || file.Length == 0)
            return BadRequest(new { message = "No file uploaded" });

        try
        {
            using var stream = file.OpenReadStream();
            var command = new UploadPlantPictureCommand(id, Guid.Parse(userId), stream, file.FileName, notes);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("pictures/{pictureId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPicture(Guid pictureId)
    {
        var picture = await _pictureRepository.GetByIdAsync(pictureId);
        if (picture == null)
            return NotFound();

        var physicalPath = _fileStorage.GetPhysicalPath(picture.FilePath);
        if (!_fileStorage.FileExists(picture.FilePath))
            return NotFound();

        var extension = Path.GetExtension(picture.FilePath).ToLowerInvariant();
        var contentType = extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            _ => "application/octet-stream"
        };

        var fileBytes = await System.IO.File.ReadAllBytesAsync(physicalPath);
        return File(fileBytes, contentType);
    }
}
