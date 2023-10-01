using System.Collections.Generic;

namespace CarBootFinderAPI.Models;

public abstract class SaleBaseModel
{
    public bool AdminApproved { get; set; }
    public string Name { get; set; }
    public LocationModel Location { get; set; }
    public string Address { get; set; }
    public string Region { get; set; }
    public List<string> DaysOpen { get; set; }
    public string Frequency { get; set; }
    public bool? OpenBankHolidays { get; set; }
    public string FromTo { get; set; }
    public string Environment { get; set; }
    public string Terrain { get; set; }
    public EntryModel Entry { get; set; }
    public bool? Toilets { get; set; }
    public bool? AccessibleToilets  { get; set; }
    public bool? Refreshments { get; set; }
    public bool? Parking { get; set; }
    public bool? AccessibleParking { get; set; }
    public string ParkingInfo { get; set; }
    public bool? PetFriendly { get; set; }
    public string OtherInfo { get; set; }
    public OrganiserDetailsModel OrganiserDetailsModel { get; set; }
    
    
}