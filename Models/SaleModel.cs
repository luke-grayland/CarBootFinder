using MongoDB.Bson;

namespace CarBootFinderAPI.Models;

public class SaleModel : SaleBaseModel
{
    public ObjectId Id { get; set; }

}