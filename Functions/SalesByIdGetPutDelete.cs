using System.IO;
using System.Threading.Tasks;
using CarBootFinderAPI.Assemblers;
using CarBootFinderAPI.Data;
using CarBootFinderAPI.Models;
using CarBootFinderAPI.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace CarBootFinderAPI.Functions;

public class SalesByIdGetPutDelete
{
    private readonly IRepository<SaleModel> _saleRepository;
    private readonly ISaleAssembler _saleAssembler;
    
    public SalesByIdGetPutDelete(ISaleAssembler saleAssembler)
    {
        var db = MongoDbUtility.GetDb();
        _saleRepository = new MongoDbRepository<SaleModel>(db, Constants.Collections.Sales);
        _saleAssembler = saleAssembler;
    }
    
    [FunctionName("SalesByIdGetPutDelete")]
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

        var sale = await _saleRepository.GetByIdAsync(id);
        return sale == null ? new NotFoundResult() : new OkObjectResult(sale); 
    }
}