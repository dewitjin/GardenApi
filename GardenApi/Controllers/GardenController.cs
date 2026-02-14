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

    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile image)
    {
        if (image == null || image.Length == 0)
        {
            return BadRequest("No image uploaded.");
        }

        // TODO: add url to bus image later.
        var imageUrl = await _plantImageService.UploadImageAsync(image);

        // // Send message to Service Bus
        // var message = new ServiceBusMessage(JsonSerializer.Serialize(new { ImageUrl = imageUrl, Action = "Review" }));
        // await _sender.SendMessageAsync(message);

        return Ok("Image uploaded and review requested.");
    }
}