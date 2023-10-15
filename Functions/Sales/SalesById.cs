using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using CarBootFinderAPI.Shared.Assemblers;
using CarBootFinderAPI.Shared.Models;
using CarBootFinderAPI.Shared.Models.Sale;
using CarBootFinderAPI.Shared.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;

namespace CarBootFinderAPI.Functions.Sales;

public class SalesById
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleAssembler _saleAssembler;
    
    public SalesById(
        ISaleAssembler saleAssembler,
        ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
        _saleAssembler = saleAssembler;
    }
    
    [FunctionName("SalesById")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "put", "delete", Route = "sales/{id}")] HttpRequest req,
        string id)
    {
        if (req.Method == HttpMethods.Put)
        {
            var reqBody = await new StreamReader(req.Body).ReadToEndAsync();
            var existingSale = await _saleRepository.GetByIdAsync(id);
            var saleInput = JsonConvert.DeserializeObject<SaleInputModel>(reqBody);
            var updatedSale = _saleAssembler.CreateSaleUpdate(saleInput, existingSale);
            
            await _saleRepository.UpdateAsync(id, updatedSale);
            return new OkResult();
        }
        
        if (req.Method == HttpMethods.Delete)
        {
            await _saleRepository.DeleteAsync(id);
            return new NoContentResult();
        }

        if (req.Method == HttpMethods.Get)
        {
            var sale = await _saleRepository.GetByIdAsync(id);
            return sale == null ? new NotFoundResult() : new OkObjectResult(sale);     
        }

        return new BadRequestErrorMessageResult("HTTP route not supported");
    }
}