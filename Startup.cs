using System;
using CarBootFinderAPI.Assemblers;
using CarBootFinderAPI.Repositories;
using CarBootFinderAPI.Utilities;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(CarBootFinderAPI.Startup))]

namespace CarBootFinderAPI;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddScoped<ISaleAssembler, SaleAssembler>();
        builder.Services.AddScoped<ISaleRepository, SaleRepository>();
        builder.Services.AddSingleton<IDatabaseService>(new DatabaseService(Environment.GetEnvironmentVariable("MongoDbConnection")));
    }
}