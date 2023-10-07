using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarBootFinderAPI.Models;

public class SaleModel : SaleBaseModel
{
    public ObjectId Id { get; set; }
    
    [BsonElement("distance")]
    public double Distance { get; set; }
    
    [BsonElement("distanceInMiles")]
    public double DistanceInMiles { get; set; }

}