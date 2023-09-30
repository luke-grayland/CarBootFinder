using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using CarBootFinderAPI.Models;
using Microsoft.AspNetCore.Mvc.Internal;
using MongoDB.Bson;

namespace CarBootFinderAPI.Assemblers;

public class SaleAssembler : ISaleAssembler
{
    public SaleModel CreateSale(SaleInputModel saleInputModel)
    {
        return new SaleModel()
        {
            Id = ObjectId.GenerateNewId(),
            Name = saleInputModel.Name,
            Refreshments = saleInputModel.Refreshments,
            LocationModel = CreateLocation(
                saleInputModel.LocationModel.Coordinates[0],
                saleInputModel.LocationModel.Coordinates[1])
        };
    }

    public SaleModel CreateSaleUpdate(SaleInputModel saleInputModel, SaleModel saleModel)
    {
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
            LocationModel = CreateLocation(0.0001d, 0.0001d),
            Region = Enum.TryParse(formInput.Region, out Constants.Region regionValue) ? regionValue : null,
            DaysOpen = ParseDays(formInput.DaysOpen),
            Frequency = CleanText(formInput.Frequency),
            OpenBankHolidays = ParseBoolNull(CleanText(formInput.OpenBankHolidays)),
            FromTo = CleanText(formInput.FromTo),
            Environment = Enum.TryParse(formInput.Region, out Constants.Environment environmentValue) ? environmentValue : null,
            Terrain = CleanText(formInput.Terrain),
            BuyerEntryTime = CleanText(formInput.BuyerEntryTime),
            BuyerEntryFee = double.Parse(formInput.BuyerEntryFee),
            SellerEntryTime = CleanText(formInput.SellerEntryTime),
            SellerEntryFee = double.Parse(formInput.SellerEntryFee),
            Toilets = ParseBoolNull(formInput.Toilets),
            AccessibleToilets = ParseBoolNull(formInput.AccessibleToilets),
            Refreshments = ParseBoolNull(formInput.Refreshments),
            Parking = ParseBoolNull(formInput.Parking),
            AccessibleParking = ParseBoolNull(formInput.AccessibleParking),
            ParkingInfo = CleanText(formInput.ParkingInfo),
            PetFriendly = ParseBoolNull(formInput.PetFriendly),
            OtherInfo = CleanText(formInput.OtherInfo),
            OrganiserDetails = SanitiseValidateOrganiserDetails(
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

    private static OrganiserDetails SanitiseValidateOrganiserDetails(
        string name, 
        string phoneNumber, 
        string publicEmail,
        string privateEmail, 
        string website,
        string facebookGroup)
    {
        return new OrganiserDetails()
        {
            Name = CleanText(name),
            PhoneNumber = int.TryParse(CleanText(phoneNumber), out var result) ? result : null,
            PublicEmailAddress = IsValidEmail(publicEmail) ? publicEmail : null,
            PrivateEmailAddress = IsValidEmail(privateEmail) ? privateEmail : null,
            Website = Uri.EscapeDataString(website),
            FacebookGroup = Uri.EscapeDataString(facebookGroup)
        };
    }

    private static string CleanText(string text)
    {
        return Regex.IsMatch(text, @"^[a-zA-Z0-9\s]+$") ? Regex.Escape(text) : "";
    }

    private static bool? ParseBoolNull(string input)
    {
        if (string.IsNullOrEmpty(input))
            return null;

        return bool.TryParse(input, out var result) ? result : null;
    }

    private static List<Constants.Days> ParseDays(IEnumerable<string> days)
    {
        var result = new List<Constants.Days>();

        foreach (var day in days)
            if (Enum.TryParse(CleanText(day), out Constants.Days matchedDay))
                result.Add(matchedDay);

        return result;
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
    

}