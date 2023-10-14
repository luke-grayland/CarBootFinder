using MongoDB.Driver;

namespace CarBootFinderAPI.Shared.Services;

public interface IDatabaseService
{
    IMongoDatabase GetDb();
}