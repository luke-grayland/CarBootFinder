namespace CarBootFinderAPI.Models;

public abstract class SaleBase
{
    public string Name { get; set; }
    public Location Location { get; set; }
    public bool? Refreshments { get; set; }
}