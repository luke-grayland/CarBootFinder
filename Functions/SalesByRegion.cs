using System.Threading.Tasks;
using System.Web.Http;
using CarBootFinderAPI.Assemblers;
using CarBootFinderAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;

namespace CarBootFinderAPI.Functions;

public class SalesByRegion
{
    private readonly ISaleRepository _saleRepository;
    
    public SalesByRegion(ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository; 
    }

    [FunctionName("SalesByRegion")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "sales/region/{region}")] HttpRequest req, 
        string region)
    {
        if (req.Method == HttpMethods.Get)
        {
            var sales = await _saleRepository.GetSalesByRegion(region);
            return new OkObjectResult(sales);    
        }
        
        return new BadRequestErrorMessageResult("HTTP route not supported");
    }
}