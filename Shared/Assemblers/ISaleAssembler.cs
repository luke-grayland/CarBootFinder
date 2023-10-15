using System.Collections.Generic;
using System.Threading.Tasks;
using CarBootFinderAPI.Shared.Models;
using CarBootFinderAPI.Shared.Models.Sale;
using CarBootFinderAPI.Shared.Models.SaleInfo;
using Microsoft.AspNetCore.Http;

namespace CarBootFinderAPI.Shared.Assemblers;

public interface ISaleAssembler
{
    SaleModel CreateSale(SaleInputModel saleInputModel);

    SaleModel CreateSaleUpdate(SaleInputModel saleInputModel, SaleModel saleModel);
    public LocationModel AssembleLocation(double longitude, double latitude);
    public SaleInputModel SanitiseValidateFormInput(IFormCollection form, string coverImageUrl);
    public List<SaleModel> CalculateDistance(IList<SaleModel> sales);
}