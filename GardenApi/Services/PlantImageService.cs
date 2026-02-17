using Azure.Storage.Blobs;
using GardenApi.Data;
using GardenApi.Services.Interfaces;
using GardenApi.Utilities.Results;


namespace GardenApi.Services;

public class PlantImageService : IImageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly GardenDbContext _context;
    private const string ContainerName = "plant-images";

    public PlantImageService(BlobServiceClient blobServiceClient, GardenDbContext context)
    {
        _blobServiceClient = blobServiceClient;
        _context = context;
    }

    /// <inheritdoc/>
    public Result ValidateImage(int plantId, IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            return Result.Failure("No image uploaded.");
        }

        if (plantId <= 0)
        {
            return Result.Failure("Invalid plant ID.");
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var extension = Path.GetExtension(image.FileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
        {
            return Result.Failure($"Invalid file type. Allowed: {string.Join(", ", allowedExtensions)}");
        }
        
        return Result.Success();
    }

    /// <inheritdoc/>
    public async Task<Result> UploadAndSaveImageAsync(int plantId, IFormFile image)
    {
        var imageUrl = await UploadImageAsync(image);
        var result = await SaveImageToDatabaseAsync(plantId, imageUrl);

        SendMessageToServiceBus(plantId, imageUrl);

        return result;
    }

    /// <inheritdoc/>
    public async Task<string> UploadImageAsync(IFormFile image)
    {
        var container = _blobServiceClient.GetBlobContainerClient(ContainerName);

        var blobName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
        var blob = container.GetBlobClient(blobName);

        await blob.UploadAsync(image.OpenReadStream(), overwrite: true);

        return blob.Uri.ToString();
    }

    /// <inheritdoc/>
    public async Task<Result> ApproveImage(int plantId)
    {
        var plant = await _context.Plants.FindAsync(plantId);
        if (plant == null)
        {
            return Result.Failure("Plant not found.");
        }

        plant.isImageApproved = true;
        await _context.SaveChangesAsync();
        return Result.Success();
    }

      /// <inheritdoc/>
    public async Task<Result> DeleteImageUpdatePlantImageName(int plantId)
    {
        var plant = await _context.Plants.FindAsync(plantId);
        if (plant == null)
        {
            return Result.Failure("Plant not found.");
        }

         // Delete the image from Azure Blob Storage before 
         // updateing the plant data.
        if (!string.IsNullOrEmpty(plant.ImageFileName))
        {
            var container = _blobServiceClient.GetBlobContainerClient(ContainerName);
            var blobName = Path.GetFileName(new Uri(plant.ImageFileName).LocalPath);
            var blob = container.GetBlobClient(blobName);
            await blob.DeleteIfExistsAsync();
        }

        plant.isImageApproved = false;
        plant.ImageFileName = null;
        await _context.SaveChangesAsync();

        return Result.Success();
    }


    /// <summary>
    /// Saves the image URL to the database for the specified plant and marks it as pending review.
    /// </summary>
    /// <param name="plantId">The ID of the plant to update.</param>
    /// <param name="imageUrl">The URL of the uploaded image.</param>
    /// <returns>A Result indicating success or failure.</returns>
    private async Task<Result> SaveImageToDatabaseAsync(int plantId, string imageUrl)
    {
        var plant = await _context.Plants.FindAsync(plantId);

        if (plant == null)
        {
            return Result.Failure("Plant not found.");
        }

        plant.ImageFileName = imageUrl;
        plant.isImageApproved = false; // Mark as pending review

        _context.Plants.Update(plant);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    /// <summary>
    /// Sends a message to the service bus to trigger the image review process. 
    /// The message includes the plant ID and image URL for the reviewer to access.
    /// </summary>
    /// <param name="plantId">The ID of the plant whose image is being reviewed.</param>
    /// <param name="imageUrl">The URL of the uploaded image.</param>
    private void SendMessageToServiceBus(int plantId, string imageUrl)
    {
        // Send message to service bus (uncomment when ready)
        // var message = new ServiceBusMessage(JsonSerializer.Serialize(new 
        // { 
        //     ImageUrl = imageUrl, 
        //     PlantId = plantId,   // ← now you can include plantId too!
        //     Action = "Review" 
        // }));
        // await _sender.SendMessageAsync(message);
    }
}