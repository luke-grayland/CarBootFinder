using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CarBootFinderAPI.Models;
using CarBootFinderAPI.Utilities;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;

namespace CarBootFinderAPI.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly IMongoCollection<SaleModel> _collection;
    
    public SaleRepository(IDatabaseService databaseService)
    {
        _collection = databaseService.GetDb().GetCollection<SaleModel>(Constants.Collections.Sales);
    }
    
    public async Task<List<SaleModel>> GetSalesByNearest(LocationModel locationModel)
    {
        return await _collection.Find(GetByNearestFilter(locationModel)).ToListAsync();
    }
    
    public async Task<SaleModel> GetByIdAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("Id cannot be null or empty");

        return await _collection.Find(GetByIdFilter(id)).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<SaleModel>> GetAllAsync()
    {
        return await _collection.Find(x => true).ToListAsync();
    }

    public async Task CreateAsync(SaleModel sale)
    {
        if (sale == null)
            throw new NullReferenceException("Sale cannot be null");
        
        await _collection.InsertOneAsync(sale);
    }

    public async Task UpdateAsync(string id, SaleModel sale)
    {
        await _collection.ReplaceOneAsync(GetByIdFilter(id), sale);
    }

    public async Task DeleteAsync(string id)
    {
        await _collection.DeleteOneAsync(GetByIdFilter(id));
    }
    
    private static FilterDefinition<SaleModel> GetByIdFilter(string id)
    {
        return Builders<SaleModel>.Filter.Eq("_id", ObjectId.Parse(id));
    }
    
    private static FilterDefinition<SaleModel> GetByNearestFilter(LocationModel locationModel)
    {
        return Builders<SaleModel>.Filter.NearSphere(x => x.LocationModel,
            new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                new GeoJson2DGeographicCoordinates(
                    locationModel.Coordinates[0],
                    locationModel.Coordinates[1])
            ));
    }
}