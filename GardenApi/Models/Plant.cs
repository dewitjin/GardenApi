namespace GardenApi.Models;

public class Plant
{
    /// <summary>
    /// Unique identifier for the plant. 
    /// This is the primary key in the database.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// What to call the plant.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The month to start planting this plant outdoors.
    /// </summary>
    public int StartMonth { get; set; }

    /// <summary>
    /// The month to finish planting this plant outdoors.
    /// </summary>
    public int EndMonth { get; set; }

    /// <summary>
    /// Any special instructions for planting this plant.
    /// </summary>
    public string? Instructions { get; set; }
}
