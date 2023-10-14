using MongoDB.Bson;

namespace CarBootFinderAPI.Shared.Models;

public class SaleModel : SaleBaseModel
{
    public ObjectId Id { get; set; }

}