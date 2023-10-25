using System.Threading.Tasks;
using System.Web.Http;
using CarBootFinderAPI.Shared.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using CarBootFinderAPI.Shared.Constants;

namespace CarBootFinderAPI.Functions.Sales;

public class SalesByPhrase
{
    private readonly ISaleRepository _saleRepository;
    
    public SalesByPhrase(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository; 
    }

    [FunctionName("SalesByPhrase")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "sales/phrase/{phrase}")] 
        HttpRequest req, 
        string phrase)
    {
        if (req.Method == HttpMethods.Get)
        {
            if (string.IsNullOrEmpty(phrase))
                return new BadRequestErrorMessageResult(Constants.ErrorMessages.PhraseParamRequired);
            
            if (!int.TryParse(req.Query["pageNumber"], out var pageNumber) || pageNumber <= 0)
                return new BadRequestErrorMessageResult(Constants.ErrorMessages.PageNumberQueryInvalid);

            var sales = await _saleRepository.GetSalesByPhrase(phrase, pageNumber);
            
            return new OkObjectResult(sales);    
        }
        
        return new NotFoundResult();
    }
}