using MongoDB.Bson;

namespace CarBootFinderAPI.Shared.Models.Sale;

public class SaleModel : SaleBaseModel
{
    public ObjectId Id { get; set; }

}