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

    [HttpGet("details", Name = "GetDetails")]
    public async Task<ActionResult<IEnumerable<Plant>>> GetDetails()
    {
        var plants = await _context.Plants.ToListAsync();
        return Ok(plants);
    }

    [HttpPost("upload/{plantId}", Name = "UploadImage")]
    public async Task<IActionResult> UploadImage(int plantId, IFormFile image)
    {
        Console.WriteLine("Upload called");  // or use ILogger
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
}