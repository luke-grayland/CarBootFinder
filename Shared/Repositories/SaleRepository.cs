using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
    
    public async Task<List<SaleModel>> GetSalesByNearest(LocationModel locationModel, int pageNumber)
    {
        var pipeline = new []
        {
            BsonDocument.Parse($"{{ $geoNear: {{ near: {{ type: 'Point', coordinates: [{locationModel.Coordinates[0]}, {locationModel.Coordinates[1]}] }}, distanceField: 'location.distanceInMeters', spherical: true }} }}"),
            BsonDocument.Parse($"{{ $match: {{ adminApproved: true }} }}"),
            BsonDocument.Parse($"{{ $skip: {GetSkipCount(pageNumber)} }}"),
            BsonDocument.Parse($"{{ $limit: {Constants.SystemSettings.PageSize} }}")
        };
    
        return await _collection.Aggregate<SaleModel>(pipeline).ToListAsync();
    }

    public async Task<List<SaleModel>> GetSalesByRegion(string region, int pageNumber)
    {
        var matchedRegion = Constants.Constants.Region.AllRegions
            .Select(x => new KeyValuePair<string,string>(x.Key.ToLower(), x.Value))
            .FirstOrDefault(x => x.Key == region.ToLower());
        
        if (matchedRegion.Key == null || matchedRegion.Value == null)
            throw new ArgumentException("Invalid region");

        var filter = GetFilterBase().And(GetAdminApprovedFilter(), GetByRegionFilter(matchedRegion.Value));
        
        return await _collection.Find(filter)
            .SortBy(x => x.Id)
            .Skip(GetSkipCount(pageNumber))
            .Limit(Constants.SystemSettings.PageSize)
            .ToListAsync();
    }

    public async Task<List<SaleModel>> GetSalesByPhrase(string phrase, int pageNumber)
    {
        var cleanPhrase = RemoveSpecialCharacters(phrase);
        
        if (string.IsNullOrWhiteSpace(cleanPhrase))
            throw new ArgumentException("Search phrase empty");
    
        var filter = GetFilterBase().And(GetAdminApprovedFilter(), GetByPhraseFilter(cleanPhrase));
    
        return await _collection.Find(filter)
            .Skip(GetSkipCount(pageNumber))
            .Limit(Constants.SystemSettings.PageSize)
            .ToListAsync();
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

    public async Task<IEnumerable<SaleModel>> GetAllAsync(int pageNumber)
    {
        return await _collection
            .Find(GetAdminApprovedFilter())
            .SortBy(x => x.Id)
            .Skip(GetSkipCount(pageNumber))
            .Limit(Constants.SystemSettings.PageSize)
            .ToListAsync();
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

    private static FilterDefinition<SaleModel> GetByPhraseFilter(string phrase)
    {
        var nameSearch = GetFilterBase()
                .Regex("name", new BsonRegularExpression(new Regex(phrase, RegexOptions.IgnoreCase)));
        
        var addressSearch = GetFilterBase()
            .Regex("address", new BsonRegularExpression(new Regex(phrase, RegexOptions.IgnoreCase)));

        return GetFilterBase().Or(nameSearch, addressSearch);
    }

    private static FilterDefinitionBuilder<SaleModel> GetFilterBase()
    {
        return Builders<SaleModel>.Filter;
    }

    private static int GetSkipCount(int pageNumber)
    {
        return pageNumber == 1 ? 0 : (pageNumber - 1) * Constants.SystemSettings.PageSize;
    }

    private static string RemoveSpecialCharacters(string input)
    {
        const string pattern = @"[^a-zA-Z0-9,\s]";

        return Regex.Replace(input, pattern, "");
    }

}