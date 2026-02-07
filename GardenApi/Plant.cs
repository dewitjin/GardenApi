namespace GardenApi;

public class Plant
{
    /// <summary>
    /// What to call the plant.
    /// </summary>
    public string? Name { get; set; }
    public int StartMonth { get; set; }
    public int EndMonth { get; set; }
    public string? Instructions { get; set; }
}
