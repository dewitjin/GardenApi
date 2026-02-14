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

   /// <summary>
   /// The filename of an image representing this plant. 
   /// This could be used to display an image in the UI.
   /// </summary>
    public string? ImageFileName { get; set; }

    /// <summary>
    /// Indicates whether the uploaded image for this plant 
    /// has been approved by a human reviewer.
    /// </summary>
    public bool isImageApproved { get; set; }   
}
