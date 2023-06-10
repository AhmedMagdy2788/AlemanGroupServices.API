using Microsoft.EntityFrameworkCore;
using AlemanGroupServices.Core.Const;
using AlemanGroupServices.Core.Repositories;
using System.Linq.Expressions;

namespace AlemanGroupServices.EF.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private MySQLDBContext _context;
        public BaseRepository(MySQLDBContext context)
        {
            _context = context;
        }

        public T Add(T entity)
        {
            _context.Set<T>().Add(entity);
            return entity;
        }

        public T Update(T entity)
        {
            _context.Set<T>().Update(entity);
            return entity;
        }

        public T Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
            return entity;
        }

        public IEnumerable<T> AddRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
            return entities;
        }

        public T? Find(Expression<Func<T, bool>> match,
            string[]? includes = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
                foreach (var include in includes) query =
                        query.Include(include);
            return query.SingleOrDefault(match);
            //return _context.Set<T>().SingleOrDefault(match);
        }

        public IEnumerable<T?> FindAll(Expression<Func<T, bool>> match,
            string[]? includes = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
                foreach (var include in includes) query =
                        query.Include(include);
            return query.Where(match).ToList();
        }

        public IEnumerable<T?> FindAll(Expression<Func<T, bool>> match,
            int? take, int? skip, Expression<Func<T, object>>? orderBy = null,
            string orderByDirection = OrderBy.Ascending)
        {
            IQueryable<T> query = _context.Set<T>();
            if (take.HasValue)
                query.Take(take.Value);
            if (skip.HasValue)
                query.Skip(skip.Value);
            if (orderBy != null)
            {
                if (orderByDirection == OrderBy.Ascending)
                    return query.Where(match).OrderBy(orderBy).ToList();
                else
                    return query.Where(match).OrderByDescending(orderBy).ToList();
            }
            return query.Where(match).ToList();
        }

        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public T? GetById(dynamic id)
        {
            return _context.Set<T>().Find(id);
        }

        public async Task<T?> GetByIdAsync(dynamic id)
        {
            return await _context.Set<T>().FindAsync(id);
        }
    }
}

