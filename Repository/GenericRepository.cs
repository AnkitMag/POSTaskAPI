
using Microsoft.EntityFrameworkCore;
using POSTaskAPI.Data;
using POSTaskAPI.RepositoryInterface;
using System.Linq.Expressions;

namespace POSTaskAPI.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDBContext db;
        private DbSet<T> table = null;
        public GenericRepository(AppDBContext ekDataContext)
        {
            db = ekDataContext;
            table = db.Set<T>();
        }
        public async Task<T> AddAsync(T obj)
        {
            await table.AddAsync(obj);
            await db.SaveChangesAsync();
            return obj;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await table.FindAsync(id);
            if (result == null)
            {
                throw new Exception("Delete the valid data");
            }
            try
            {
                db.Remove(result);
                await db.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, object>> include = null)
        {
            IQueryable<T> query = table;

            if (include != null)
                query = query.Include(include);

            return await query.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {

            var result = await table.FindAsync(id);
            return result;
        }

        public async Task<T> GetByValueAsync(string fieldName, string checkValue)
        {

            var result = await table.FirstOrDefaultAsync(x => EF.Property<string>(x, fieldName) == checkValue);
            return result;
        }

        public async Task<T> UpdateAsync(T obj, int id)
        {
            var updatedData = db.Update(obj);
            await db.SaveChangesAsync();
            return updatedData.Entity;
        }

        public async Task<bool> AnyAsync(int id)
        {
            var result = await table.AnyAsync(x => EF.Property<int>(x, "Id") == id);
            return result;
        }

        public async Task<bool> AnyAsync(string fieldName, string checkValue)
        {
            var result = await table.AnyAsync(x => EF.Property<string>(x, fieldName) == checkValue);
            return result;
        }
    }
}
