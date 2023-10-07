using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CarBootFinderAPI.Models
{
    public class LocationModel
    {
        [BsonElement("type")]
        [BsonRepresentation(BsonType.String)]
        public string Type { get; set; } = "Point";

        [BsonElement("coordinates")]
        public double[] Coordinates { get; set; }
    }
}