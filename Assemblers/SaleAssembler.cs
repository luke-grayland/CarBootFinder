using CarBootFinderAPI.Models;
using MongoDB.Bson;

namespace CarBootFinderAPI.Assemblers;

public class SaleAssembler : ISaleAssembler
{
    public SaleModel CreateSale(SaleInputModel saleInputModel)
    {
        return new SaleModel()
        {
            Id = ObjectId.GenerateNewId(),
            Name = saleInputModel.Name
        };
    }

    public SaleModel CreateSaleUpdate(SaleInputModel saleInputModel, SaleModel saleModel)
    {
        if (!string.IsNullOrEmpty(saleInputModel.Name))
            saleModel.Name = saleInputModel.Name;

        if (saleInputModel.Refreshments != null)
            saleModel.Refreshments = (bool)saleInputModel.Refreshments;

        return saleModel;
    }
    
}