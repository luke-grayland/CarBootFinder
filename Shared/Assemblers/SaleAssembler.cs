using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using CarBootFinderAPI.Shared.Models.Sale;
using CarBootFinderAPI.Shared.Models.SaleInfo;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;

namespace CarBootFinderAPI.Shared.Assemblers;

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
            BankHolidayAdditionalInfo = saleInputModel.BankHolidayAdditionalInfo,
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
            OrganiserDetails = saleInputModel.OrganiserDetails,
            CoverImageUrl = saleInputModel.CoverImageUrl
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

    public List<SaleModel> CalculateDistance(IList<SaleModel> sales)
    {
        foreach (var sale in sales)
            if (sale.Location.DistanceInMeters != 0)
                sale.Location.DistanceInMiles = 
                    sale.Location.DistanceInMeters * Constants.Constants.Search.MeterToMileMultiplier;
            
        return sales.ToList();
    }
    
    public LocationModel AssembleLocation(double longitude, double latitude)
    {
        return CreateLocation(longitude, latitude);
    }

    public SaleInputModel SanitiseValidateFormInput(IFormCollection form, string coverImageUrl)
    {
        return new SaleInputModel()
        {
            AdminApproved = false,
            Name = CleanText(form["Name"]),
            Location = CreateLocation(
                double.Parse(form["Longitude"]), 
                double.Parse(form["Latitude"])),
            Address = SanitiseAddress(form["Address"]),
            Region = ParseRegion(CleanText(form["Region"])),
            DaysOpen = ParseDays(form["DaysOpen"]),
            Frequency = CleanText(form["Frequency"]),
            OpenBankHolidays = ParseBoolNull(CleanText(form["OpenBankHolidays"])),
            BankHolidayAdditionalInfo = CleanText(form["BankHolidayAdditionalInfo"]),
            FromTo = CleanText(form["FromTo"]),
            Environment = ParseEnvironment(CleanText(form["Environment"])),
            Terrain = CleanText(form["Terrain"]),
            Entry = ParseEntry(
                form["BuyerEntryTime"], 
                form["BuyerEntryFee"], 
                form["SellerEntryTime"], 
                form["SellerEntryFee"],
                form["ClosingTime"]),
            Toilets = ParseBoolNull(form["Toilets"]),
            AccessibleToilets = ParseBoolNull(form["AccessibleToilets"]),
            Refreshments = ParseBoolNull(form["Refreshments"]),
            Parking = ParseBoolNull(form["Parking"]),
            AccessibleParking = ParseBoolNull(form["AccessibleParking"]),
            ParkingInfo = CleanText(form["ParkingAdditionalInfo"]),
            PetFriendly = ParseBoolNull(form["PetFriendly"]),
            OtherInfo = CleanText(form["OtherInfo"]),
            OrganiserDetails = SanitiseValidateOrganiserDetails(
                form["OrganiserName"],
                form["OrganiserPhoneNumber"],
                form["OrganisePublicEmailAddress"],
                form["OrganiserPrivateEmailAddress"],
                form["Website"],
                form["FacebookGroup"]),
            CoverImageUrl = coverImageUrl
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
        return string.IsNullOrEmpty(text) 
            ? null 
            : Regex.Replace(text, @"[^a-zA-Z0-9:\s,\-]", "");
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

    private static List<string> ParseDays(string daysInput)
    {
        var days = daysInput.Split(",").Select(CleanText).ToList();
        return days.Where(day => Constants.Constants.Days.AllDays.Contains(day)).ToList();
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
        return Constants.Constants.Region.AllRegions.Any(x => x.Value == region) ? region : null;
    }

    private static string ParseEnvironment(string environment)
    {
        return Constants.Constants.Environment.AllEnvironments.Contains(environment) ? environment : null;
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
        string sellerEntryFee,
        string closingTime)
    {
        return new EntryModel()
        {
            BuyerEntryTime = CleanText(buyerEntryTime),
            BuyerEntryFee = ParseFee(buyerEntryFee),
            SellerEntryTime = CleanText(sellerEntryTime),
            SellerEntryFee = ParseFee(sellerEntryFee),
            ClosingTime = CleanText(closingTime)
        };
    }
    
    private static string SanitiseAddress(string address)
    {
        const string pattern = @"[^a-zA-Z0-9\s,.\-]";
        return Regex.Replace(address, pattern, "");
    }
}