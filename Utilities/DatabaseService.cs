using MongoDB.Driver;

namespace CarBootFinderAPI.Utilities;

public class DatabaseService : IDatabaseService
{
    private readonly MongoClient _mongoClient;

    public DatabaseService(string connectionString)
    {
        _mongoClient = new MongoClient(connectionString);
    }

    public IMongoDatabase GetDb()
    {
        return _mongoClient.GetDatabase("CarBootFinder");
    }
}