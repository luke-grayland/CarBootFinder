using CarBootFinderAPI.Models;

namespace CarBootFinderAPI.Assemblers;

public interface ISaleAssembler
{
    SaleModel CreateSale(SaleInputModel saleInputModel);

    SaleModel CreateSaleUpdate(SaleInputModel saleInputModel, SaleModel saleModel);
    public Location CreateLocation(float longitude, float latitude);
}