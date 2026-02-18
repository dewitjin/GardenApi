namespace GardenApi.Services.Interfaces;

using GardenApi.Utilities.Results;
using Microsoft.AspNetCore.Mvc;
public interface IImageService
{
    /// <summary>
    /// Validates the uploaded image for the given plant ID. This could include checks like:
    /// - Ensuring the plant ID exists in the database.
    /// - Validating the image format (e.g., only allowing JPEG or PNG).
    /// </summary>
    /// <param name="plantId">The ID of the plant for which the image is being validated.</param>
    /// <param name="image">The image to validate.</param>
    /// <returns>A Result object indicating success or failure of the validation.</returns>
    Result ValidateImage(int plantId, IFormFile image);

    /// <summary>
    /// Uploads the image to Azure Blob Storage, generates a review SAS URL, 
    /// and saves the image information to the database.    
    /// This method also sends a message to the service bus to trigger the image review process.
    /// </summary>
    /// <param name="plantId">The ID of the plant to associate the image with.</param>
    /// <param name="image">The image to upload.</param>
    /// <returns>A Result object indicating success or failure of the upload and save operation.</returns>
    Task<Result> UploadAndSaveImageAsync(int plantId, IFormFile image);

    /// <summary>
    /// Uploads the image to Azure Blob Storage and returns the URL of the uploaded image.
    /// </summary>
    /// <param name="image">The image to upload.</param>
    /// <returns>The URL of the uploaded image.</returns>
    Task<string> UploadImageAsync(IFormFile image);

    /// <summary>
    /// Approves the uploaded image for the specified plant 
    /// by updating the database record to mark the image as approved.
    /// </summary>
    /// <param name="plantId">The ID of the plant whose image is to be approved.</param>
    /// <returns>A Result object indicating success or failure of the approval operation.</returns>
    Task<Result> ApproveImage(int plantId);

    /// <summary>
    /// Deletes the uploaded image for the specified plant by removing the image from Azure Blob Storage
    /// and updating the database record to remove the image URL and mark it as not approved.
    /// </summary>
    /// <param name="plantId">The ID of the plant whose image is to be deleted.</param>
    /// <returns>A Result object indicating success or failure of the deletion operation.</returns>
    Task<Result> DeleteImageUpdatePlantImageName(int plantId);
}