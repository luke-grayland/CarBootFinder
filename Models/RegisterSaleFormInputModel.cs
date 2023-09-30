using System.Collections.Generic;

namespace CarBootFinderAPI.Models;

public class RegisterSaleFormInputModel
{
    public string Name { get; set; }
    public string Longitude { get; set; }
    public string Latitude { get; set; }
    public string Region { get; set; }
    public string Description { get; set; }
    public List<string> DaysOpen { get; set; }
    public string Frequency { get; set; }
    public string OpenBankHolidays { get; set; }
    public string FromTo { get; set; }
    public string Environment { get; set; }
    public string Terrain { get; set; }
    public string BuyerEntryTime { get; set; }
    public string BuyerEntryFee { get; set; }
    public string SellerEntryTime { get; set; }
    public string SellerEntryFee { get; set; }
    public string Toilets { get; set; }
    public string AccessibleToilets  { get; set; }
    public string Refreshments { get; set; }
    public string Parking { get; set; }
    public string AccessibleParking { get; set; }
    public string ParkingInfo { get; set; }
    public string PetFriendly { get; set; }
    public string OrganiserName { get; set; }
    public string OrganiserPhoneNumber { get; set; }
    public string OrganiserPublicEmailAddress { get; set; }
    public string OrganiserPrivateEmailAddress { get; set; }
    public string Website { get; set; }
    public string FacebookGroup { get; set; }
    public string OtherInfo { get; set; }
}
