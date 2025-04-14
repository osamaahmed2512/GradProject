using GraduationProject.data;
using Microsoft.EntityFrameworkCore;
using Polly;
using System.Linq.Expressions;

namespace GraduationProject.Services
{
    public class GenaricRepository<T> : IGenaricRepository<T> where T : class
    {  
        private readonly AppDBContext _Context;

        public GenaricRepository(AppDBContext dBContext)
        {
            _Context = dBContext;
        }

        public T GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _Context.Set<T>().FindAsync(id);
        }

        public IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            IQueryable<T> query = _Context.Set<T>();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return query.Where(criteria).ToList();
        }
        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            IQueryable<T> query = _Context.Set<T>();

            if (includes != null)
                foreach (var include in includes)
                    query = query.Include(include);

            return await query.Where(criteria).ToListAsync();
        }
        public void Add(T entity)
        {
            _Context.Set<T>().Add(entity);
            _Context.SaveChanges();
        }

        public async void AddAsync(T entity)
        {
            await _Context.Set<T>().AddAsync(entity);
            await _Context.SaveChangesAsync();
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _Context.Set<T>().AddRange(entities);
            _Context.SaveChanges();
        }

        public async Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate)
        {
             return await _Context.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public async Task Delete(T entity)
        {
             _Context.Set<T>().Remove(entity);
             await _Context.SaveChangesAsync();
        }

        public async Task DeleteRange(IEnumerable<T> entities)
        {
            _Context.Set<T>().RemoveRange(entities);
            await _Context.SaveChangesAsync();
        }
    }
}
