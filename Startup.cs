using System;
using CarBootFinderAPI.Shared.Assemblers;
using CarBootFinderAPI.Shared.Repositories;
using CarBootFinderAPI.Shared.Services;
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
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddSingleton<IDatabaseService>(new DatabaseService(Environment.GetEnvironmentVariable("MongoDbConnection")));
        builder.Services.AddSingleton<FileService>();
    }
}