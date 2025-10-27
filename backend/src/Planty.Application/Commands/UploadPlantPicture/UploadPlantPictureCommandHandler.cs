namespace Planty.Application.Commands.UploadPlantPicture;

using MediatR;
using Planty.Application.Interfaces;
using Planty.Application.Services;
using Planty.Contracts.Plants;
using Planty.Domain.Entities;
using Planty.Domain.Repositories;

public class UploadPlantPictureCommandHandler : IRequestHandler<UploadPlantPictureCommand, PlantPictureResponse>
{
    private readonly IPlantRepository _plantRepository;
    private readonly IPlantPictureRepository _pictureRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IPermissionService _permissionService;
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private const long MaxFileSize = 10 * 1024 * 1024; // 10MB

    public UploadPlantPictureCommandHandler(
        IPlantRepository plantRepository,
        IPlantPictureRepository pictureRepository,
        IFileStorageService fileStorageService,
        IPermissionService permissionService)
    {
        _plantRepository = plantRepository;
        _pictureRepository = pictureRepository;
        _fileStorageService = fileStorageService;
        _permissionService = permissionService;
    }

    public async Task<PlantPictureResponse> Handle(UploadPlantPictureCommand request, CancellationToken cancellationToken)
    {
        // Validate plant exists
        var plant = await _plantRepository.GetByIdAsync(request.PlantId, cancellationToken);
        if (plant == null)
        {
            throw new InvalidOperationException("Plant not found");
        }

        // Check if user has permission to perform care actions (including taking pictures)
        if (!await _permissionService.CanUserCarePlantAsync(request.PlantId, request.UserId, cancellationToken))
        {
            throw new UnauthorizedAccessException("You don't have permission to upload pictures for this plant.");
        }

        // Validate file extension
        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            throw new Exception($"Invalid file type. Allowed types: {string.Join(", ", AllowedExtensions)}");
        }

        // Validate file size
        if (request.FileStream.Length > MaxFileSize)
        {
            throw new Exception($"File size exceeds maximum allowed size of {MaxFileSize / 1024 / 1024}MB");
        }

        // Save the file
        var filePath = await _fileStorageService.SavePlantPictureAsync(
            request.FileStream, 
            request.FileName, 
            cancellationToken);

        // Create picture record
        var picture = new PlantPicture
        {
            PlantId = request.PlantId,
            FilePath = filePath,
            Notes = request.Notes,
            TakenAt = DateTime.UtcNow
        };

        await _pictureRepository.AddAsync(picture, cancellationToken);
        await _pictureRepository.SaveChangesAsync(cancellationToken);

        return new PlantPictureResponse(
            picture.Id,
            picture.TakenAt,
            $"/api/plants/pictures/{picture.Id}",
            picture.Notes
        );
    }
}
