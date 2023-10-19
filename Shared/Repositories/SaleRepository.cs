using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarBootFinderAPI.Shared.Models.Sale;
using CarBootFinderAPI.Shared.Models.SaleInfo;
using CarBootFinderAPI.Shared.Services;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CarBootFinderAPI.Shared.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly IMongoCollection<SaleModel> _collection;

    public SaleRepository(IDatabaseService databaseService)
    {
        _collection = databaseService.GetDb().GetCollection<SaleModel>(Constants.Constants.Collections.Sales);
    }
    
    public async Task<List<SaleModel>> GetSalesByNearest(LocationModel locationModel)
    {
        var pipeline = new []
        {
            BsonDocument.Parse($"{{ $geoNear: {{ near: {{ type: 'Point', coordinates: [{locationModel.Coordinates[0]}, {locationModel.Coordinates[1]}] }}, distanceField: 'location.distanceInMeters', spherical: true }} }}"),
            BsonDocument.Parse($"{{ $match: {{ adminApproved: true }} }}")
        };

        return await _collection.Aggregate<SaleModel>(pipeline).ToListAsync();
    }

    public async Task<List<SaleModel>> GetSalesByRegion(string region)
    {
        var matchedRegion = Constants.Constants.Region.AllRegions
            .Select(x => new KeyValuePair<string,string>(x.Key.ToLower(), x.Value))
            .FirstOrDefault(x => x.Key == region.ToLower());
        
        if (matchedRegion.Key == null || matchedRegion.Value == null)
            throw new ArgumentException("Invalid region");

        var filter = GetFilterBase().And(GetAdminApprovedFilter(), GetByRegionFilter(matchedRegion.Value));
        
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<List<SaleModel>> GetUnapprovedSales()
    {
        var getUnapprovedSalesFilter = GetFilterBase().Eq("adminApproved", false);
        return await _collection.Find(getUnapprovedSalesFilter).ToListAsync();
    }

    public async Task<List<SaleModel>> CheckDuplicateSales(IEnumerable<SaleModel> unapprovedSales)
    {
        var duplicateSales = new List<SaleModel>();
        
        foreach (var unapprovedSale in unapprovedSales)
        {
            var filter = GetFilterBase().And(GetAdminApprovedFilter(), MatchDuplicateFilter(unapprovedSale));
            var matches = await _collection.Find(filter).ToListAsync();

            if (!matches.Any()) continue;

            foreach (var match in matches.Where(match => duplicateSales.All(x => x.Id != match.Id)))
                duplicateSales.Add(match);
        }

        return duplicateSales;
    }

    public async Task<SaleModel> GetByIdAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("Id cannot be null or empty");

        var filter = GetFilterBase().And(GetByIdFilter(id), GetAdminApprovedFilter());
        
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<SaleModel>> GetAllAsync()
    {
        return await _collection.Find(GetAdminApprovedFilter()).ToListAsync();
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
        return GetFilterBase().Eq("_id", ObjectId.Parse(id));
    }

    private static FilterDefinition<SaleModel> GetByRegionFilter(string region)
    {
        return GetFilterBase().Eq("region", region);
    }
    
    private static FilterDefinition<SaleModel> MatchDuplicateFilter(SaleBaseModel unapprovedSale)
    {
        var filterBuilder = GetFilterBase();
        var filters = new List<FilterDefinition<SaleModel>> { GetAdminApprovedFilter() };

        var orFilters = new List<FilterDefinition<SaleModel>>();

        if (!string.IsNullOrEmpty(unapprovedSale.Name))
            orFilters.Add(filterBuilder.Eq("name", unapprovedSale.Name));

        if (!string.IsNullOrEmpty(unapprovedSale.OrganiserDetails?.PhoneNumber))
            orFilters.Add(filterBuilder.Eq("organiserDetails.phoneNumber", 
                unapprovedSale.OrganiserDetails.PhoneNumber));

        if (!string.IsNullOrEmpty(unapprovedSale.OrganiserDetails?.PublicEmailAddress))
            orFilters.Add(filterBuilder.Eq("organiserDetails.publicEmailAddress", 
                unapprovedSale.OrganiserDetails.PublicEmailAddress));

        if (!string.IsNullOrEmpty(unapprovedSale.OrganiserDetails?.PrivateEmailAddress))
            orFilters.Add(filterBuilder.Eq("organiserDetails.privateEmailAddress", 
                unapprovedSale.OrganiserDetails.PrivateEmailAddress));

        if (!string.IsNullOrEmpty(unapprovedSale.Address))
            orFilters.Add(filterBuilder.Eq("address", unapprovedSale.Address));

        if (orFilters.Any())
            filters.Add(filterBuilder.Or(orFilters));
        
        var combinedFilter = filterBuilder.And(filters);

        return combinedFilter;
    }

    private static FilterDefinition<SaleModel> GetAdminApprovedFilter()
    {
        return GetFilterBase().Eq("adminApproved", true);
    }

    private static FilterDefinitionBuilder<SaleModel> GetFilterBase()
    {
        return Builders<SaleModel>.Filter;
    }


}