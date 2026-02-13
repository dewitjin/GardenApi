using Microsoft.AspNetCore.Mvc;

namespace GardenApi.Controllers;

[ApiController]
[Route("[controller]/plants")]
public class GardenController : ControllerBase
{
    // Static list of plants.
    // Months are 1=January to 12=December.
    // These are typical safe outdoor start/end ranges (direct sow or transplant).
    private static readonly List<Plant> Plants = new()
    {
        new Plant
        {
            Name = "Eggplant",
            StartMonth = 4, // April (transplant after last frost ~late March/early April)
            EndMonth = 5,   // May (plant by mid-May for long warm season),
            Instructions = "Use special fertilizer."
        },
        new Plant
        {
            Name = "Carrot",
            StartMonth = 2, // February (early spring direct sow, frost-tolerant)
            EndMonth = 3,   // March (main spring window; or later for fall crop)
            Instructions = "Use deep planters."
        },
        new Plant
        {
            Name = "Tulip",
            StartMonth = 9,  // September (start of fall bulb planting)
            EndMonth = 11,    // November (best Oct-Nov when soil cools)
            Instructions = "Use special fertilizer."
        },
          new Plant
        {
            Name = "Daffodils",
            StartMonth = 9,  // September (start of fall bulb planting)
            EndMonth = 11,    // November (best Oct-Nov when soil cools)
            Instructions = "Use special fertilizer."
        },
        new Plant
        {
            Name = "Garlic",
            StartMonth = 10, // October (main fall planting for big bulbs)
            EndMonth = 11,    // November (up to mid/late Nov works in mild coast)
            Instructions = "Use big planters."
        },
           new Plant
        {
            Name = "Onions",
            StartMonth = 10, // October (main fall planting for big bulbs)
            EndMonth = 11,    // November (up to mid/late Nov works in mild coast)
            Instructions = "Use big planters."
        }
    };

    [HttpGet(Name = "GetPlants")]
    public ActionResult<IEnumerable<Plant>> Get()
    {
        var summaries = Plants.Select(p => new
        {
            p.Name,
            p.StartMonth,
            p.EndMonth
            // Instructions is automatically excluded
        });

        return Ok(summaries);
    }

    [HttpGet("details", Name = "GetDetails")]
    public ActionResult<IEnumerable<Plant>> GetDetails()
    {
        return Plants;
    }
}
