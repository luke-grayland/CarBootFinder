using MongoDB.Bson;

namespace CarBootFinderAPI.Models;

public class SaleModel : SaleBase
{
    public ObjectId Id { get; set; }

}