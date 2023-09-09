using AlemanGroupServices.Core.Const;
using System.Linq.Expressions;

namespace AlemanGroupServices.Core.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        T? GetById(dynamic id);
        Task<T?> GetByIdAsync(dynamic id);
        IEnumerable<T> GetAll();
        public IEnumerable<T> GetRange(int range, Expression<Func<T, object>> orderBy, int offset = 0, string orderByDirection = OrderBy.Ascending);
        T? Find(Expression<Func<T, bool>> match,
            string[]? includes = null);
        IEnumerable<T?> FindAll(Expression<Func<T, bool>> match,
            string[]? includes = null);
        IEnumerable<T?> FindAll(Expression<Func<T, bool>> match,
            int? take, int? skip, Expression<Func<T, object>>? orderBy = null,
            string orderByDirection = OrderBy.Ascending);
        Task<bool> AnyAsync(Expression<Func<T, bool>> match);
        public bool Any(Expression<Func<T, bool>> match);
        T Add(T entity);
        T Update(T entity);
        T Delete(T entity);
        IEnumerable<T> AddRange(IEnumerable<T> entities);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
    }
}
