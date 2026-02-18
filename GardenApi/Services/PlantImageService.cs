using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using GardenApi.Data;
using GardenApi.Services.Interfaces;
using GardenApi.Utilities.Results;


namespace GardenApi.Services;

public class PlantImageService : IImageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly GardenDbContext _context;
    private const string ContainerName = "plant-images";
    private readonly ServiceBusSender _serviceBusSender;

    public PlantImageService(BlobServiceClient blobServiceClient, GardenDbContext context, ServiceBusSender serviceBusSender)
    {
        _blobServiceClient = blobServiceClient;
        _context = context;
        _serviceBusSender = serviceBusSender;
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
        // 1. Upload to blob storage → get permanent URL
        var imageUrl = await UploadImageAsync(image);

        // 2. Generate SAS URL for review (e.g., 30 days)
        var blobClient = _blobServiceClient
            .GetBlobContainerClient(ContainerName)
            .GetBlobClient(Path.GetFileName(imageUrl));  // extract blob name from URL

        var reviewSasUrl = await GenerateReadOnlySasUrlAsync(blobClient, TimeSpan.FromDays(30));

        // 3. Save to database (both URLs)
        var result = await SaveImageToDatabaseAsync(plantId, imageUrl, reviewSasUrl);
        if (!result.IsSuccess)
        {
            // Optional: delete the blob if DB save fails (cleanup)
            await blobClient.DeleteIfExistsAsync();
            return result;
        }

        await SendMessageToServiceBusAsync(plantId, reviewSasUrl);

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
    /// Saves the image URL and review SAS URL to the database for the specified plant.
    /// </summary>
    /// <param name="plantId">The ID of the plant to update.</param>
    /// <param name="imageUrl">The URL of the uploaded image.</param>
    /// <param name="reviewSasUrl">The SAS URL for the uploaded image for review.</param>
    /// <returns>A Result indicating success or failure.</returns>
    private async Task<Result> SaveImageToDatabaseAsync(int plantId, string imageUrl, string reviewSasUrl)
    {
        var plant = await _context.Plants.FindAsync(plantId);

        if (plant == null)
        {
            return Result.Failure("Plant not found.");
        }

        plant.ImageFileName = imageUrl;
        plant.ReviewImageSasUrl = reviewSasUrl; // Store the same URL for review access
        plant.isImageApproved = false; // Mark as pending review

        _context.Plants.Update(plant);
        await _context.SaveChangesAsync();

        return Result.Success();
    }

    /// <summary>
    /// Sends a message to the service bus to trigger the image review process. 
    /// The message includes the plant ID and SAS URL for the reviewer to access.
    /// </summary>
    /// <param name="plantId">The ID of the plant whose image is being reviewed.</param>
    /// <param name="reviewSasUrl">The SAS URL of the uploaded image for review.</param>
    private async Task SendMessageToServiceBusAsync(int plantId, string reviewSasUrl)
    {
        var message = new ServiceBusMessage(JsonSerializer.Serialize(new 
        { 
            ReviewSasUrl = reviewSasUrl, 
            PlantId = plantId,
            Action = "Review" 
        }));
        
        await _serviceBusSender.SendMessageAsync(message);
    }


    /// <summary>
    /// Generates a read-only read-only Shared Access Signature (SAS) 
    /// SAS URL for the specified blob with a defined validity period.
    /// </summary>
    /// <param name="blob">The Azure BlobClient instance for which to generate the SAS URL.</param>
    /// <param name="validityPeriod">The validity period for the SAS URL (default is 30 days).</param>
    /// <returns>The generated read-only SAS URL as a string.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the blob client is not authorized to generate SAS URIs.</exception>
    private async Task<string> GenerateReadOnlySasUrlAsync(BlobClient blob, TimeSpan validityPeriod = default)
    {
        // Default to 30 days if not specified
        validityPeriod = validityPeriod == default ? TimeSpan.FromDays(30) : validityPeriod;

        if (!blob.CanGenerateSasUri)
        {
            throw new InvalidOperationException("BlobClient is not authorized to generate SAS URIs. " +
                                                "Ensure it's created with SharedKeyCredential or appropriate permissions.");
        }

        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = blob.BlobContainerName,
            BlobName = blob.Name,
            Resource = "b",  // 'b' = blob
            StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),  // small buffer for clock skew
            ExpiresOn = DateTimeOffset.UtcNow.Add(validityPeriod),
        };

        sasBuilder.SetPermissions(BlobSasPermissions.Read);  // read-only

        // Optional: restrict to HTTPS only (recommended)
        sasBuilder.Protocol = SasProtocol.Https;

        // Generate the SAS URI (includes the token)
        var sasUri = blob.GenerateSasUri(sasBuilder);

        return sasUri.ToString();
    }
}