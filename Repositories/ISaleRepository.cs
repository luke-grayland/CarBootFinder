using System.Collections.Generic;
using System.Threading.Tasks;
using CarBootFinderAPI.Models;

namespace CarBootFinderAPI.Repositories;

public interface ISaleRepository
{
    Task<SaleModel> GetByIdAsync(string id);
    Task<IEnumerable<SaleModel>> GetAllAsync();
    Task CreateAsync(SaleModel sale);
    Task UpdateAsync(string id, SaleModel sale);
    Task DeleteAsync(string id);
    Task<List<SaleModel>> GetSalesByNearest(Location location);
}