using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CarBootFinderAPI.Data;

public class MongoDbRepository<T> : IRepository<T>
{
    private readonly IMongoCollection<T> _collection;

    public MongoDbRepository(IMongoDatabase db, string collectionName)
    {
        _collection = db.GetCollection<T>(collectionName);
    }

    public async Task<T> GetByIdAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("Id cannot be null or empty");

        return await _collection.Find(GetByIdFilter(id)).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(x => true).ToListAsync();
    }

    public async Task CreateAsync(T entity)
    {
        if (entity == null)
            throw new NullReferenceException("Entity cannot be null");
        
        await _collection.InsertOneAsync(entity);
    }

    public async Task UpdateAsync(string id, T entity)
    {
        await _collection.ReplaceOneAsync(GetByIdFilter(id), entity);
    }

    public async Task DeleteAsync(string id)
    {
        await _collection.DeleteOneAsync(GetByIdFilter(id));
    }
    
    private static FilterDefinition<T> GetByIdFilter(string id)
    {
        return Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
    }
}