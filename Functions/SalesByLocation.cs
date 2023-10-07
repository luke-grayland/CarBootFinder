using System.Threading.Tasks;
using CarBootFinderAPI.Assemblers;
using CarBootFinderAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;

namespace CarBootFinderAPI.Functions;

public class SalesByLocation
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleAssembler _saleAssembler;
    
    public SalesByLocation(
        ISaleAssembler saleAssembler,
        ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository; 
        _saleAssembler = saleAssembler;
    }
    
    [FunctionName("SalesByLocation")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "sales/near/{longitude}/{latitude}")] HttpRequest req, 
        float longitude, 
        float latitude)
    {
        var location = _saleAssembler.AssembleLocation(longitude, latitude);
        var results = await _saleRepository.GetSalesByNearest(location);
        var sales = _saleAssembler.CalculateDistance(results);
        return new OkObjectResult(sales);
    }
}