using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using CarBootFinderAPI.Shared.Models.Sale;

namespace CarBootFinderAPI.Shared.Services;

public class EmailService : IEmailService
{
    public SmtpClient GetSmtpClient()
    {
        return new SmtpClient(Environment.GetEnvironmentVariable("SmtpServer"))
        {
            Port = Constants.SystemSettings.SmtpServerPort,
            Credentials = new NetworkCredential(
                Environment.GetEnvironmentVariable("SmtpUsername"),
                Environment.GetEnvironmentVariable("SmtpPassword")),
            EnableSsl = true,
        };
    }

    public MailMessage CreateUnapprovedSalesEmail(IList<SaleModel> unapprovedSales, IList<SaleModel> duplicateSales)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress("CarBootFinder@carbootfinderapp.com"),
            Subject = $"{unapprovedSales.Count} Unapproved Car Boot(s) - {DateTime.Now}",
            Body = CreateEmailBody(unapprovedSales, duplicateSales),
            IsBodyHtml = true,
        };

        mailMessage.To.Add(Constants.SystemSettings.AdminEmailAddress);
        
        return mailMessage;
    }

    private static string CreateEmailBody(IEnumerable<SaleModel> unapprovedSales, IList<SaleModel> duplicateSales)
    {
        var emailBody = new StringBuilder();

        foreach (var duplicateSale in duplicateSales)
        {
            emailBody.AppendLine($"Potential Duplicate: {duplicateSale.Name} - {duplicateSale.Id}");
            emailBody.AppendLine("<br />");
        }
        
        if (duplicateSales.Any())
            emailBody.AppendLine("<br />");
        
        foreach (var unapprovedSale in unapprovedSales)
        {
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Name: {unapprovedSale.Name}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Coordinates: {unapprovedSale.Location.Coordinates[0]}, {unapprovedSale.Location.Coordinates[1]}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Address: {unapprovedSale.Address}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Region: {unapprovedSale.Region}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Days Open: {string.Join(",", unapprovedSale.DaysOpen)}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Frequency: {unapprovedSale.Frequency}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Open Bank Holidays: {unapprovedSale.OpenBankHolidays}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Bank Holiday Additional Info: {unapprovedSale.BankHolidayAdditionalInfo}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"From/To: {unapprovedSale.FromTo}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Environment: {unapprovedSale.Environment}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Terrain: {unapprovedSale.Terrain}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Buyer Entry Time: {unapprovedSale.Entry.BuyerEntryTime}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Buyer Entry Fee: {unapprovedSale.Entry.BuyerEntryFee}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Seller Entry Time: {unapprovedSale.Entry.SellerEntryTime}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Seller Entry Fee: {unapprovedSale.Entry.SellerEntryFee}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Closing Time: {unapprovedSale.Entry.ClosingTime}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Toilets: {unapprovedSale.Toilets}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Accessible Toilets: {unapprovedSale.AccessibleToilets}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Refreshments: {unapprovedSale.Refreshments}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Parking: {unapprovedSale.Parking}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Accessible Parking: {unapprovedSale.AccessibleParking}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Parking Info: {unapprovedSale.ParkingInfo}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Pet Friendly: {unapprovedSale.PetFriendly}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Other Info: {unapprovedSale.OtherInfo}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Organiser Name: {unapprovedSale.OrganiserDetails.Name}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Organiser Number: {unapprovedSale.OrganiserDetails.PhoneNumber}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Organiser Public Email: {unapprovedSale.OrganiserDetails.PublicEmailAddress}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Organiser Private Email: {unapprovedSale.OrganiserDetails.PrivateEmailAddress}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Website: {unapprovedSale.OrganiserDetails.Website}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Facebook Group: {unapprovedSale.OrganiserDetails.FacebookGroup}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine($"Cover Image URL: {unapprovedSale.CoverImageUrl}");
            emailBody.AppendLine("<br />");
            emailBody.AppendLine("<br />");
        }

        return emailBody.ToString();
    }
    
}