using System.Threading.Tasks;
using CarBootFinderAPI.Assemblers;
using CarBootFinderAPI.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;

namespace CarBootFinderAPI.Functions;

public class SalesByIdGetPutDelete
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleAssembler _saleAssembler;
    
    public SalesByIdGetPutDelete(
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
        var location = _saleAssembler.CreateLocation(longitude, latitude);
        
        await _saleRepository.GetSalesByNearest(location);
        return new OkResult();
    }
}