using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarBootFinderAPI.Data;

public interface IRepository<T>
{
    Task<T> GetByIdAsync(string id);
    Task<IEnumerable<T>> GetAllAsync();
    Task CreateAsync(T entity);
    Task UpdateAsync(string id, T entity);
    Task DeleteAsync(string id);
}