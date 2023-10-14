using MongoDB.Bson.Serialization.Attributes;

namespace CarBootFinderAPI.Shared.Models.SaleInfo;

public class CoverImage
{
    [BsonElement("fileName")]
    public string Filename { get; set; }
    
    [BsonElement("data")]
    public byte[] Data { get; set; }
}