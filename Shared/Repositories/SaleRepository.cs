using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarBootFinderAPI.Shared.Models;
using CarBootFinderAPI.Shared.Models.SaleInfo;
using CarBootFinderAPI.Shared.Services;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CarBootFinderAPI.Shared.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly IMongoCollection<SaleModel> _collection;
    private const double MaxDistanceInRadians = 
        SystemSettings.SearchMaxDistanceKilometers / Constants.Search.EarthRadiusKilometers;
    
    public SaleRepository(IDatabaseService databaseService)
    {
        _collection = databaseService.GetDb().GetCollection<SaleModel>(Constants.Collections.Sales);
    }
    
    public async Task<List<SaleModel>> GetSalesByNearest(LocationModel locationModel)
    {
        var pipeline = new BsonDocument[]
        {
            BsonDocument.Parse($"{{ $geoNear: {{ near: {{ type: 'Point', coordinates: [{locationModel.Coordinates[0]}, {locationModel.Coordinates[1]}] }}, distanceField: 'location.distanceInMeters', spherical: true }} }}")
        };

        return await _collection.Aggregate<SaleModel>(pipeline).ToListAsync();
    }

    public async Task<List<SaleModel>> GetSalesByRegion(string region)
    {
        var matchedRegion = Constants.Region.AllRegions
            .Select(x => new KeyValuePair<string,string>(x.Key.ToLower(), x.Value))
            .FirstOrDefault(x => x.Key == region);
        
        if (matchedRegion.Key == null || matchedRegion.Value == null)
            throw new ArgumentException("Invalid region");

        return await _collection.Find(GetByRegionFilter(matchedRegion.Value)).ToListAsync();
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

    private static FilterDefinition<SaleModel> GetByRegionFilter(string region)
    {
        return Builders<SaleModel>.Filter.Eq("region", region);
    }
    
}