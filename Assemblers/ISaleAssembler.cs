using System.Collections.Generic;
using CarBootFinderAPI.Models;

namespace CarBootFinderAPI.Assemblers;

public interface ISaleAssembler
{
    SaleModel CreateSale(SaleInputModel saleInputModel);

    SaleModel CreateSaleUpdate(SaleInputModel saleInputModel, SaleModel saleModel);
    public LocationModel AssembleLocation(double longitude, double latitude);
    public SaleInputModel SanitiseValidateFormInput(RegisterSaleFormInputModel formInput);
    public List<SaleModel> CalculateDistance(IList<SaleModel> sales);
}