using AlemanGroupServices.Core.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AlemanGroupServices.Core.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        T? GetById(dynamic id);
        Task<T?> GetByIdAsync(dynamic id);
        IEnumerable<T> GetAll();
        T? Find(Expression<Func<T, bool>> match,
            string[]? includes = null);
        IEnumerable<T?> FindAll(Expression<Func<T, bool>> match,
            string[]? includes = null);
        IEnumerable<T?> FindAll(Expression<Func<T, bool>> match,
            int? take, int? skip, Expression<Func<T, object>>? orderBy = null,
            string orderByDirection = OrderBy.Ascending);
        T Add(T entity);
        T Update(T entity);
        T Delete(T entity);
        IEnumerable<T> AddRange(IEnumerable<T> entities);
    }
}
