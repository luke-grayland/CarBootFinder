using MongoDB.Bson.Serialization.Attributes;

namespace CarBootFinderAPI.Shared.Models.SaleInfo;

public class OrganiserDetailsModel
{
    [BsonElement("name")]
    public string Name { get; set; }
    
    [BsonElement("phoneNumber")]
    public string PhoneNumber { get; set; }
    
    [BsonElement("publicEmailAddress")]
    public string PublicEmailAddress { get; set; }
    
    [BsonElement("privateEmailAddress")]
    public string PrivateEmailAddress { get; set; }
    
    [BsonElement("website")]
    public string Website { get; set; }
    
    [BsonElement("facebookGroup")]
    public string FacebookGroup { get; set; }
}