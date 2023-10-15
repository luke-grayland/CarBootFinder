using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using CarBootFinderAPI.Shared.Repositories;
using CarBootFinderAPI.Shared.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace CarBootFinderAPI.Functions.Watchers;

public class RegistrationWatcher
{
    private readonly ISaleRepository _saleRepository;
    private readonly IEmailService _emailService;
    
    public RegistrationWatcher(ISaleRepository saleRepository, IEmailService emailService)
    {
        _saleRepository = saleRepository;
        _emailService = emailService;
    }
    
    [FunctionName("RegistrationWatcher")]
    public async Task RunAsync([TimerTrigger("0 0 7 * * *")] TimerInfo myTimer, ILogger log)
    {
        var unapprovedSales = await _saleRepository.GetUnapprovedSales();

        if (!unapprovedSales.Any())
            return;
        
        var smtpClient = _emailService.GetSmtpClient();
        var email = _emailService.CreateUnapprovedSalesEmail(unapprovedSales);
        
        try
        {
            smtpClient.Send(email);
            Console.WriteLine("Email sent successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email: {ex.Message}");
        }
    }
}