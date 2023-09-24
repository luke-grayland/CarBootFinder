using MongoDB.Bson;

namespace CarBootFinderAPI.Models;

public class SaleModel
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public bool Refreshments { get; set; }
}