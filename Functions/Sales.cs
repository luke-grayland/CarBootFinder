using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
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
            var coverImage = req.Form.Files["CoverImage"];

            var saleInput = await _saleAssembler.SanitiseValidateFormInput(
                JsonConvert.DeserializeObject<RegisterSaleFormInputModel>(reqBody), coverImage);
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