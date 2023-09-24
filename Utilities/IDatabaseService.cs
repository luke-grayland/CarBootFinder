using MongoDB.Driver;

namespace CarBootFinderAPI.Utilities;

public interface IDatabaseService
{
    IMongoDatabase GetDb();
}