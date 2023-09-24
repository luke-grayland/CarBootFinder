using MongoDB.Bson.Serialization.Attributes;

namespace CarBootFinderAPI.Models;

public class Location
{
    public string Type { get; } = "Point";
    
    [BsonElement("Coordinates")]
    public float[] Coordinates { get; set; }
}
