using System.Linq.Expressions;

namespace GraduationProject.Services
{
    public interface IGenaricRepository<T> where T : class
    {
        T GetById(int id);
        Task<T> GetByIdAsync(int id);
        IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, string[] includes = null);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[] includes = null);
        void Add(T entity);
        void AddAsync(T entity);
        void AddRange(IEnumerable<T> entities);

        Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate);
        Task Delete(T entity);
        Task DeleteRange(IEnumerable<T> entities);
    }
}
