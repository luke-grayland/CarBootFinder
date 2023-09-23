using MongoDB.Bson;

namespace CarBootFinderAPI;

public class SaleModel
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
}