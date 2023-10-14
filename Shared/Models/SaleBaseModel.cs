using System.Collections.Generic;
using CarBootFinderAPI.Shared.Models.SaleInfo;
using MongoDB.Bson.Serialization.Attributes;

namespace CarBootFinderAPI.Shared.Models;

public abstract class SaleBaseModel
{
    [BsonElement("adminApproved")]
    public bool AdminApproved { get; set; }
    
    [BsonElement("name")]
    public string Name { get; set; }
    
    [BsonElement("location")]
    public LocationModel Location { get; set; }
    
    [BsonElement("address")]
    public string Address { get; set; }
    
    [BsonElement("region")]
    public string Region { get; set; }
    
    [BsonElement("daysOpen")]
    public List<string> DaysOpen { get; set; }
    
    [BsonElement("frequency")]
    public string Frequency { get; set; }
    
    [BsonElement("openBankHolidays")]
    public bool? OpenBankHolidays { get; set; }
    
    [BsonElement("bankHolidayAdditionalInfo")]
    public string BankHolidayAdditionalInfo { get; set; }
    
    [BsonElement("fromTo")]
    public string FromTo { get; set; }
    
    [BsonElement("environment")]
    public string Environment { get; set; }
    
    [BsonElement("terrain")]
    public string Terrain { get; set; }
    
    [BsonElement("entry")]
    public EntryModel Entry { get; set; }
    
    [BsonElement("toilets")]
    public bool? Toilets { get; set; }
    
    [BsonElement("accessibleToilets")]
    public bool? AccessibleToilets  { get; set; }
    
    [BsonElement("refreshments")]
    public bool? Refreshments { get; set; }
    
    [BsonElement("parking")]
    public bool? Parking { get; set; }
    
    [BsonElement("accessibleParking")]
    public bool? AccessibleParking { get; set; }
    
    [BsonElement("parkingInfo")]
    public string ParkingInfo { get; set; }
    
    [BsonElement("petFriendly")]
    public bool? PetFriendly { get; set; }
    
    [BsonElement("otherInfo")]
    public string OtherInfo { get; set; }
    
    [BsonElement("organiserDetails")]
    public OrganiserDetailsModel OrganiserDetails { get; set; }
    
    [BsonElement("coverImage")]
    public CoverImage CoverImage { get; set; }   
}