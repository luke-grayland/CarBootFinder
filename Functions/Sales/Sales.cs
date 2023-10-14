using System.Threading.Tasks;
using System.Web.Http;
using CarBootFinderAPI.Shared.Assemblers;
using CarBootFinderAPI.Shared.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace CarBootFinderAPI.Functions.Sales;

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
            var form = await req.ReadFormAsync();
            var saleInput = await _saleAssembler.SanitiseValidateFormInput(form);
            var createdSale = _saleAssembler.CreateSale(saleInput);
            
            await _saleRepository.CreateAsync(createdSale);
            return new CreatedResult("/sales", createdSale);
        }

        if (req.Method == HttpMethods.Get)
        {
            var sales = await _saleRepository.GetAllAsync();
            return new OkObjectResult(sales);    
        }

        return new BadRequestErrorMessageResult("HTTP route not supported");
    }
}