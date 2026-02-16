using System.Linq.Expressions;

namespace POSTaskAPI.RepositoryInterface
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> AddAsync(T obj);
        Task<List<T>> GetAllAsync(Expression<Func<T, object>> include = null);
        Task<T> GetByIdAsync(int id);
        Task<T> GetByValueAsync(string fieldName, string checkValue);
        Task<T> UpdateAsync(T obj, int id);
        Task<bool> DeleteAsync(int id);
        Task<bool> AnyAsync(int id);
        Task<bool> AnyAsync(string fieldName, string checkValue);
    }
}
