namespace Planty.Infrastructure.Services;

using Planty.Application.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _uploadDirectory;
    private const string PlantPicturesFolder = "plant-pictures";

    public FileStorageService(string uploadsBasePath)
    {
        // Use the configured uploads path from appsettings
        // For development: data/uploads (relative to ContentRootPath)
        // For production: /opt/planty-data/uploads (absolute path)
        var dataPath = Path.Combine(uploadsBasePath, PlantPicturesFolder);
        _uploadDirectory = dataPath;
        
        // Ensure directory exists
        if (!Directory.Exists(_uploadDirectory))
        {
            Directory.CreateDirectory(_uploadDirectory);
        }
    }

    public async Task<string> SavePlantPictureAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        // Generate unique filename to avoid conflicts
        var fileExtension = Path.GetExtension(fileName);
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var relativePath = Path.Combine(PlantPicturesFolder, uniqueFileName);
        var fullPath = Path.Combine(_uploadDirectory, uniqueFileName);

        // Save the file
        using (var fileStreamOutput = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            await fileStream.CopyToAsync(fileStreamOutput, cancellationToken);
        }

        // Return relative path for storage in database
        return relativePath;
    }

    public string GetPhysicalPath(string relativePath)
    {
        // Convert relative path to physical path
        var fileName = Path.GetFileName(relativePath);
        return Path.Combine(_uploadDirectory, fileName);
    }

    public bool FileExists(string relativePath)
    {
        var physicalPath = GetPhysicalPath(relativePath);
        return File.Exists(physicalPath);
    }
}
