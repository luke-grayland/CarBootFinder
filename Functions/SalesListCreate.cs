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

public class SalesListCreate
{
    private readonly IRepository<SaleModel> _saleRepository;
    private readonly ISaleAssembler _saleAssembler;

    public SalesListCreate(ISaleAssembler saleAssembler)
    {
        var db = MongoDbUtility.GetDb();
        _saleRepository = new MongoDbRepository<SaleModel>(db, Constants.Collections.Sales);
        _saleAssembler = saleAssembler;
    }
    
    [FunctionName("SalesListCreate")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "sales")] HttpRequest req)
    {
        if (req.Method == HttpMethods.Post)
        {
            var reqBody = await new StreamReader(req.Body).ReadToEndAsync();
            var saleInput = JsonConvert.DeserializeObject<SaleInputModel>(reqBody);
            var createdSale = _saleAssembler.CreateSale(saleInput);
            
            await _saleRepository.CreateAsync(createdSale);
            return new CreatedResult("/sales", createdSale);
        }

        var sales = await _saleRepository.GetAllAsync();
        return new OkObjectResult(sales);
    }
}