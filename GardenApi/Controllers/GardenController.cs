using GardenApi.Data;
using GardenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GardenApi.Services.Interfaces;

namespace GardenApi.Controllers;

[ApiController]
[Route("[controller]/plants")]
public class GardenController : ControllerBase
{
    private readonly GardenDbContext _context;
    private readonly IImageService _plantImageService;

    public GardenController(GardenDbContext context, IImageService imageService)
    {
        _context = context;
        _plantImageService = imageService;
    }

    /// <summary>
    /// This endpoint returns a list of all plants in the database,
    /// but only includes the Name, StartMonth, and EndMonth properties in the response.
    /// </summary>
    /// <returns>A list of plant summaries with only the specified properties.</returns>
    [HttpGet(Name = "GetPlants")]
    public async Task<IActionResult> Get()
    {
        var summaries = await _context.Plants
            .Select(p => new
            {
                p.Name,
                p.StartMonth,
                p.EndMonth
                // Instructions excluded automatically
            })
            .ToListAsync();

        return Ok(summaries);
    }

    /// <summary>
    /// This endpoint returns a list of all plants in the database, including all properties.
    /// </summary>
    /// <returns>A list of all plants in the database.</returns>
    [HttpGet("details", Name = "GetDetails")]
    public async Task<ActionResult<IEnumerable<Plant>>> GetDetails()
    {
        var plants = await _context.Plants.ToListAsync();
        return Ok(plants);
    }

    /// <summary>
    /// This endpoint allows clients to upload an image for a specific plant.
    /// It performs validation on the uploaded image and, if valid, uploads it to Azure Blob
    /// Storage, saves the image URL to the database, and sends a message to the service bus to trigger the image review process.
    /// </summary>
    /// <param name="plantId">The ID of the plant for which the image is being uploaded.</param>
    /// <param name="image">The image file to be uploaded.</param>
    /// <returns>Ok with a success message if the upload is successful.</returns>
    [HttpPost("upload/{plantId}", Name = "UploadImage")]
    public async Task<IActionResult> UploadImage(int plantId, IFormFile image)
    {
        var result = _plantImageService.ValidateImage(plantId, image);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        result = await _plantImageService.UploadAndSaveImageAsync(plantId, image);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok("Image uploaded and review requested.");
    }

    /// <summary>
    /// This endpoint allows an admin to approve the uploaded image for a specific plant.
    /// It updates the database record to mark the image as approved.
    /// </summary>
    /// <param name="plantId">The ID of the plant whose image is to be approved.</param>
    /// <returns>Ok with a success message.</returns>
    [HttpPut("approve/{plantId}", Name = "ApproveImage")]
    public async Task<IActionResult> ApproveImage(int plantId)
    {
        var result = await _plantImageService.ApproveImage(plantId);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok("Image approved.");
    }

    /// <summary>
    /// This endpoint allows an admin to delete the uploaded image for a specific plant.
    /// It removes the image from Azure Blob Storage and updates the database record to remove the image URL and mark it as not approved.
    /// </summary>
    /// <param name="plantId">The ID of the plant whose image is to be deleted.</param>
    /// <returns>Ok with a success message.</returns>
    [HttpDelete("delete/{plantId}", Name = "DeleteImage")]
    public async Task<IActionResult> DeleteImage(int plantId)
    {
        var result = await _plantImageService.DeleteImageUpdatePlantImageName(plantId);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }
        return Ok("Image deleted.");
    }

    /// <summary>
    /// This endpoint is intended to be used by human reviewers 
    /// when they click the approval link in the email notification.
    /// It performs the same logic as the ApproveImage endpoint, 
    /// but is designed to be accessed via a simple GET request from an email link, 
    /// rather than a PUT request from the admin dashboard. 
    /// </summary>
    /// <param name="plantId">The ID of the plant whose image is to be approved.</param>
    /// <returns>Ok with a success message.</returns>
    [HttpGet("review/approve/{plantId}")]
    public async Task<IActionResult> ReviewApprove(int plantId)
    {
        // Same logic as ApproveImage
        var result = await _plantImageService.ApproveImage(plantId);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        // TODO: Optionally redirect to a thank you page or return a message.
        return Ok("Image approved via review link.");

    }

    /// <summary>
    /// This endpoint is intended to be used by human reviewers
    /// when they click the deletion link in the email notification.
    /// It performs the same logic as the DeleteImage endpoint,
    /// but is designed to be accessed via a simple GET request from an email link,
    /// rather than a DELETE request from the admin dashboard.
    /// </summary>
    /// <param name="plantId">The ID of the plant whose image is to be deleted.</param>
    /// <returns>Ok with a success message.</returns>
    [HttpGet("review/delete/{plantId}")]
    public async Task<IActionResult> ReviewDelete(int plantId)
    {
        // same logic as DeleteImage
        var result = await _plantImageService.DeleteImageUpdatePlantImageName(plantId);
        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        // TODO: Optionally redirect to a thank you page or return a message.
        return Ok("Image deleted via review link.");
    }
}