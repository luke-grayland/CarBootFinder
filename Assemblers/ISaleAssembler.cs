using System.Collections.Generic;
using System.Threading.Tasks;
using CarBootFinderAPI.Models;
using Microsoft.AspNetCore.Http;

namespace CarBootFinderAPI.Assemblers;

public interface ISaleAssembler
{
    SaleModel CreateSale(SaleInputModel saleInputModel);

    SaleModel CreateSaleUpdate(SaleInputModel saleInputModel, SaleModel saleModel);
    public LocationModel AssembleLocation(double longitude, double latitude);
    public Task<SaleInputModel> SanitiseValidateFormInput(RegisterSaleFormInputModel formInput, IFormFile coverImage);
    public List<SaleModel> CalculateDistance(IList<SaleModel> sales);
}