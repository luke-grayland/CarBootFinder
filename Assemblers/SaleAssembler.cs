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
}