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
            Name = saleInputModel.Name,
            Refreshments = saleInputModel.Refreshments,
            Location = new Location()
            {
                Coordinates = new float[]
                {
                    saleInputModel.Location.Coordinates[0],
                    saleInputModel.Location.Coordinates[1]
                }
            }
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

    public Location CreateLocation(float longitude, float latitude)
    {
        return new Location()
        {
            Coordinates = new float[] { longitude, latitude }
        };
    }
}