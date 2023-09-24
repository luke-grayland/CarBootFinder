using System.IO;
using System.Threading.Tasks;
using CarBootFinderAPI.Assemblers;
using CarBootFinderAPI.Repositories;
using CarBootFinderAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace CarBootFinderAPI.Functions;

public class Sales
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleAssembler _saleAssembler;

    public Sales(
        ISaleAssembler saleAssembler,
        ISaleRepository saleRepository)
    {
        _saleRepository = saleRepository;
        _saleAssembler = saleAssembler;
    }
    
    [FunctionName("Sales")]
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