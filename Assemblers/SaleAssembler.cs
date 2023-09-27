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
            LocationModel = CreateLocation(
                saleInputModel.LocationModel.Coordinates[0],
                saleInputModel.LocationModel.Coordinates[1])
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

    public LocationModel AssembleLocation(double longitude, double latitude)
    {
        return CreateLocation(longitude, latitude);
    }

    private static LocationModel CreateLocation(double longitude, double latitude)
    {
        return new LocationModel()
        {
            Coordinates = new double[] { longitude, latitude }
        };
    }
}