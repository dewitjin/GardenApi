using Azure.Storage.Blobs;
using GardenApi.Services.Interfaces;

namespace GardenApi.Services;

public class PlantImageService : IImageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private const string ContainerName = "plant-images";

    public PlantImageService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    /// <summary>
    /// Uploads the image to Azure Blob Storage and returns the URL of the uploaded image.
    /// </summary>
    /// <param name="image">The image to upload.</param>
    /// <returns>The URL of the uploaded image.</returns>
    public async Task<string> UploadImageAsync(IFormFile image)
    {
        if (image == null || image.Length == 0)
            throw new ArgumentException("No image");

        var container = _blobServiceClient.GetBlobContainerClient(ContainerName);

        var blobName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
        var blob = container.GetBlobClient(blobName);

        await blob.UploadAsync(image.OpenReadStream(), overwrite: true);

        return blob.Uri.ToString();
    }
}