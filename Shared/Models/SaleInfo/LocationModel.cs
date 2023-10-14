using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarBootFinderAPI.Shared.Models.SaleInfo
{
    public class LocationModel
    {
        [BsonElement("type")]
        [BsonRepresentation(BsonType.String)]
        public string Type { get; set; } = "Point";

        [BsonElement("coordinates")]
        public double[] Coordinates { get; set; }
        
        [BsonElement("distanceInMeters")]
        public double DistanceInMeters { get; set; }
    
        [BsonElement("distanceInMiles")]
        public double DistanceInMiles { get; set; }
    }
}