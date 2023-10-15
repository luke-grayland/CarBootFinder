using System.Collections.Generic;
using System.Net.Mail;
using CarBootFinderAPI.Shared.Models.Sale;

namespace CarBootFinderAPI.Shared.Services;

public interface IEmailService
{
    SmtpClient GetSmtpClient();
    MailMessage CreateUnapprovedSalesEmail(IList<SaleModel> unapprovedSales);
}