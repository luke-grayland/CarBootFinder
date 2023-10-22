using System.Threading.Tasks;
using System.Web.Http;
using CarBootFinderAPI.Shared.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using CarBootFinderAPI.Shared.Constants;

namespace CarBootFinderAPI.Functions.Sales;

public class SalesByRegion
{
    private readonly ISaleRepository _saleRepository;
    
    public SalesByRegion(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository; 
    }

    [FunctionName("SalesByRegion")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "sales/region/{region}")] 
        HttpRequest req, 
        string region)
    {
        if (req.Method == HttpMethods.Get)
        {
            if (string.IsNullOrEmpty(region))
                return new BadRequestErrorMessageResult(Constants.ErrorMessages.RegionParameterRequired);
            
            if (!int.TryParse(req.Query["pageNumber"], out var pageNumber) || pageNumber <= 0)
                return new BadRequestErrorMessageResult(Constants.ErrorMessages.PageNumberQueryInvalid);

            var sales = await _saleRepository.GetSalesByRegion(region, pageNumber);
            
            return new OkObjectResult(sales);    
        }
        
        return new NotFoundResult();
    }
}