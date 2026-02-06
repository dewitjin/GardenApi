using Microsoft.AspNetCore.Mvc;

namespace GardenApi.Controllers;

[ApiController]
[Route("[controller]/plants")]
public class GardenController : ControllerBase
{
    // Static list of example plants with realistic Burnaby, BC planting months.
    // Months are 1=January to 12=December.
    // These are typical safe outdoor start/end ranges (direct sow or transplant).
    private static readonly List<Plant> Plants = new()
{
    new Plant
    {
        Name = "Eggplant",
        StartMonth = 4, // April (transplant after last frost ~late March/early April)
        EndMonth = 5   // May (plant by mid-May for long warm season)
    },
    new Plant
    {
        Name = "Carrot",
        StartMonth = 2, // February (early spring direct sow, frost-tolerant)
        EndMonth = 3   // March (main spring window; or later for fall crop)
    },
    new Plant
    {
        Name = "Tulip",
        StartMonth = 9,  // September (start of fall bulb planting)
        EndMonth = 11    // November (best Oct-Nov when soil cools)
    },
    new Plant
    {
        Name = "Garlic",
        StartMonth = 10, // October (main fall planting for big bulbs)
        EndMonth = 11    // November (up to mid/late Nov works in mild coast)
    }
};

    [HttpGet(Name = "GetPlants")]
    public IEnumerable<Plant> Get()
    {
        return Plants;
    }
}
