using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using CarBootFinderAPI.Models;
using MongoDB.Bson;

namespace CarBootFinderAPI.Assemblers;

public class SaleAssembler : ISaleAssembler
{
    public SaleModel CreateSale(SaleInputModel saleInputModel)
    {
        return new SaleModel()
        {
            Id = ObjectId.GenerateNewId(),
            AdminApproved = saleInputModel.AdminApproved,
            Name = saleInputModel.Name,
            Location = saleInputModel.Location,
            Address = saleInputModel.Address,
            Region = saleInputModel.Region,
            DaysOpen = saleInputModel.DaysOpen,
            Frequency = saleInputModel.Frequency,
            OpenBankHolidays = saleInputModel.OpenBankHolidays,
            FromTo = saleInputModel.FromTo,
            Environment = saleInputModel.Environment,
            Terrain = saleInputModel.Terrain,
            Entry = saleInputModel.Entry,
            Toilets = saleInputModel.Toilets,
            AccessibleToilets = saleInputModel.AccessibleToilets,
            Refreshments = saleInputModel.Refreshments,
            Parking = saleInputModel.Parking,
            AccessibleParking = saleInputModel.AccessibleParking,
            ParkingInfo = saleInputModel.ParkingInfo,
            PetFriendly = saleInputModel.PetFriendly,
            OtherInfo = saleInputModel.OtherInfo,
            OrganiserDetailsModel = saleInputModel.OrganiserDetailsModel
        };
    }

    public SaleModel CreateSaleUpdate(SaleInputModel saleInputModel, SaleModel saleModel)
    {
        //method WIP
        
        if (!string.IsNullOrEmpty(saleInputModel.Name))
            saleModel.Name = saleInputModel.Name;

        if (saleInputModel.Refreshments != null)
            saleModel.Refreshments = (bool)saleInputModel.Refreshments;

        return saleModel;
    }

    public LocationModel AssembleLocation(double longitude, double latitude)
    {
        return CreateLocation(longitude, latitude);
    }

    public SaleInputModel SanitiseValidateFormInput(RegisterSaleFormInputModel formInput)
    {
        return new SaleInputModel()
        {
            AdminApproved = false,
            Name = CleanText(formInput.Name),
            Location = CreateLocation(
                double.Parse(formInput.Longitude), 
                double.Parse(formInput.Latitude)),
            Address = SanitiseAddress(formInput.Address),
            Region = ParseRegion(CleanText(formInput.Region)),
            DaysOpen = ParseDays(formInput.DaysOpen),
            Frequency = CleanText(formInput.Frequency),
            OpenBankHolidays = ParseBoolNull(CleanText(formInput.OpenBankHolidays)),
            FromTo = CleanText(formInput.FromTo),
            Environment = ParseEnvironment(CleanText(formInput.Environment)),
            Terrain = CleanText(formInput.Terrain),
            Entry = ParseEntry(
                formInput.BuyerEntryTime, 
                formInput.BuyerEntryFee, 
                formInput.SellerEntryTime, 
                formInput.SellerEntryFee),
            Toilets = ParseBoolNull(formInput.Toilets),
            AccessibleToilets = ParseBoolNull(formInput.AccessibleToilets),
            Refreshments = ParseBoolNull(formInput.Refreshments),
            Parking = ParseBoolNull(formInput.Parking),
            AccessibleParking = ParseBoolNull(formInput.AccessibleParking),
            ParkingInfo = CleanText(formInput.ParkingAdditionalInfo),
            PetFriendly = ParseBoolNull(formInput.PetFriendly),
            OtherInfo = CleanText(formInput.OtherInfo),
            OrganiserDetailsModel = SanitiseValidateOrganiserDetails(
                formInput.OrganiserName,
                formInput.OrganiserPhoneNumber,
                formInput.OrganiserPublicEmailAddress,
                formInput.OrganiserPrivateEmailAddress,
                formInput.Website,
                formInput.FacebookGroup)
        };
    }
    
    private static LocationModel CreateLocation(double longitude, double latitude)
    {
        return new LocationModel()
        {
            Coordinates = new double[] { longitude, latitude }
        };
    }

    private static OrganiserDetailsModel SanitiseValidateOrganiserDetails(
        string name, 
        string phoneNumber, 
        string publicEmail,
        string privateEmail, 
        string website,
        string facebookGroup)
    {
        return new OrganiserDetailsModel()
        {
            Name = CleanText(name),
            PhoneNumber = CleanText(phoneNumber),
            PublicEmailAddress = IsValidEmail(publicEmail) ? publicEmail : null,
            PrivateEmailAddress = IsValidEmail(privateEmail) ? privateEmail : null,
            Website = Uri.EscapeDataString(website),
            FacebookGroup = Uri.EscapeDataString(facebookGroup)
        };
    }

    private static string CleanText(string text)
    {
        if (string.IsNullOrEmpty(text))
            return null;
        
        return Regex.IsMatch(text, @"^[a-zA-Z0-9:\s,]+$") ? text : "";
    }

    private static bool? ParseBoolNull(string input)
    {
        if (string.IsNullOrEmpty(input))
            return null;

        return input.ToLower() switch
        {
            "yes" => true,
            "no" => false,
            _ => null
        };
    }

    private static List<string> ParseDays(IEnumerable<string> days)
    {
        return days.Where(day => Constants.Days.AllDays.Contains(day)).ToList();
    }
    
    private static bool IsValidEmail(string email)
    { 
        try
        { 
            var emailAddress = new MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string ParseRegion(string region)
    {
        return Constants.Region.AllRegions.Contains(region) ? region : null;
    }

    private static string ParseEnvironment(string environment)
    {
        return Constants.Environment.AllEnvironments.Contains(environment) ? environment : null;
    }

    private static double ParseFee(string fee)
    {
        if (fee.StartsWith("£"))
            fee = fee.TrimStart('£');

        return double.TryParse(fee, out var result) ? result : 0d;
    }

    private static EntryModel ParseEntry(
        string buyerEntryTime,
        string buyerEntryFee,
        string sellerEntryTime,
        string sellerEntryFee)
    {
        return new EntryModel()
        {
            BuyerEntryTime = CleanText(buyerEntryTime),
            BuyerEntryFee = ParseFee(buyerEntryFee),
            SellerEntryTime = CleanText(sellerEntryTime),
            SellerEntryFee = ParseFee(sellerEntryFee),
        };
    }
    
    private static string SanitiseAddress(string address)
    {
        const string pattern = @"[^a-zA-Z0-9\s,.\-]";
        return Regex.Replace(address, pattern, "");
    }
}