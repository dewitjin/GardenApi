using GardenApi.Data;     // For GardenDbContext
using GardenApi.Models;   // For Plant
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GardenApi.Controllers;

[ApiController]
[Route("[controller]/plants")]
public class GardenController : ControllerBase
{
    private readonly GardenDbContext _context;

    public GardenController(GardenDbContext context)
    {
        _context = context;
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
}