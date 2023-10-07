using MongoDB.Bson.Serialization.Attributes;

namespace CarBootFinderAPI.Models;

public class EntryModel
{
    [BsonElement("buyerEntryTime")]
    public string BuyerEntryTime { get; set; }
    
    [BsonElement("buyerEntryFee")]
    public double BuyerEntryFee { get; set; }
    
    [BsonElement("sellerEntryTime")]
    public string SellerEntryTime { get; set; }
    
    [BsonElement("sellerEntryFee")]
    public double SellerEntryFee { get; set; }
    
    [BsonElement("closingTime")]
    public string ClosingTime { get; set; }
}