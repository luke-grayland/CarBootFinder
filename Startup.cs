using CarBootFinderAPI.Assemblers;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(CarBootFinderAPI.Startup))]

namespace CarBootFinderAPI;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddScoped<ISaleAssembler, SaleAssembler>();
    }
}