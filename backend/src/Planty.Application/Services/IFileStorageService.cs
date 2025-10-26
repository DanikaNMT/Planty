namespace Planty.Application.Services;

public interface IFileStorageService
{
    /// <summary>
    /// Saves an uploaded file and returns the relative path
    /// </summary>
    Task<string> SavePlantPictureAsync(Stream fileStream, string fileName, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets the physical file path from a relative path
    /// </summary>
    string GetPhysicalPath(string relativePath);
    
    /// <summary>
    /// Checks if a file exists
    /// </summary>
    bool FileExists(string relativePath);
}
