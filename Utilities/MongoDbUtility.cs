using System;
using MongoDB.Driver;

namespace CarBootFinderAPI.Utilities;

public static class MongoDbUtility
{
    public static IMongoDatabase GetDb()
    {
        return new MongoClient(Environment.GetEnvironmentVariable("MongoDbConnection"))
            .GetDatabase("CarBootFinder");
    }
}