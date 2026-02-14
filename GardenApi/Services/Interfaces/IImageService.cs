namespace GardenApi.Services.Interfaces;

public interface IImageService
{
    /// <summary>
    /// Uploads the image to Azure Blob Storage and returns the URL of the uploaded image.
    /// </summary>
    /// <param name="image">The image to upload.</param>
    /// <returns>The URL of the uploaded image.</returns>
    Task<string> UploadImageAsync(IFormFile image);
}