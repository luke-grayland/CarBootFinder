using System.Collections.Generic;
using System.Threading.Tasks;
using CarBootFinderAPI.Shared.Models;
using CarBootFinderAPI.Shared.Models.Sale;
using CarBootFinderAPI.Shared.Models.SaleInfo;

namespace CarBootFinderAPI.Shared.Repositories;

public interface ISaleRepository
{
    Task<SaleModel> GetByIdAsync(string id);
    Task<IEnumerable<SaleModel>> GetAllAsync(int pageNumber);
    Task CreateAsync(SaleModel sale);
    Task UpdateAsync(string id, SaleModel sale);
    Task DeleteAsync(string id);
    Task<List<SaleModel>> GetSalesByNearest(LocationModel locationModel, int pageNumber);
    Task<List<SaleModel>> GetSalesByRegion(string region, int pageNumber);
    Task<List<SaleModel>> GetUnapprovedSales();
    Task<List<SaleModel>> CheckDuplicateSales(IEnumerable<SaleModel> unapprovedSales);
}