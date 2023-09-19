using AlemanGroupServices.Core.Const;
using AlemanGroupServices.Core.Repositories;
using Microsoft.EntityFrameworkCore;
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

        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }
        public IEnumerable<T> GetRange(int range, Expression<Func<T, object>> orderBy, int offset = 0, string orderByDirection = OrderBy.Ascending)
        {
            IQueryable<T> query = _context.Set<T>();
            if (orderByDirection == OrderBy.Ascending)
                query = query.OrderBy(orderBy);
            else
                query = query.OrderByDescending(orderBy);
            query = query.Skip(offset).Take(range);
            return query.ToList();
        }

        public async Task<IEnumerable<T>> GetRangeAsync(int range, Expression<Func<T, object>> orderBy, int offset = 0, string orderByDirection = OrderBy.Ascending)
        {
            IQueryable<T> query = _context.Set<T>();
            if (orderByDirection == OrderBy.Ascending)
                query = query.OrderBy(orderBy);
            else
                query = query.OrderByDescending(orderBy);
            query = query.Skip(offset).Take(range);
            return await query.ToListAsync();
        }

        public T? GetById(dynamic id)
        {
            return _context.Set<T>().Find(id);
        }

        public async Task<T?> GetByIdAsync(dynamic id)
        {
            return await _context.Set<T>().FindAsync(id);
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

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
            return entities;
        }

        public T? Find(Expression<Func<T, bool>> match, string[]? includes = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
                foreach (var include in includes) query = query.Include(include);
            return query.SingleOrDefault(match);
        }

        public Task<T?> FindAsync(Expression<Func<T, bool>> match, string[]? includes = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
                foreach (var include in includes) query = query.Include(include);
            return query.SingleOrDefaultAsync(match);
        }

        public IEnumerable<T?> FindAll(Expression<Func<T, bool>> match, string[]? includes = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includes != null)
                foreach (var include in includes) query =
                        query.Include(include);
            return query.Where(match).ToList();
        }

        public IEnumerable<T?> FindAll(Expression<Func<T, bool>> match, int? take, int? skip, Expression<Func<T, object>>? orderBy = null, string orderByDirection = OrderBy.Ascending)
        {
            IQueryable<T> query = _context.Set<T>();
            if (take.HasValue)
                query.Take(take.Value);
            if (skip.HasValue)
                query.Skip(skip.Value);
            if (orderBy != null)
            {
                if (orderByDirection == OrderBy.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }
            return query.Where(match).ToList();
        }

        public async Task<IEnumerable<T?>> FindAllAsync(Expression<Func<T, bool>> match, int? take, int? skip, Expression<Func<T, object>>? orderBy = null, string orderByDirection = OrderBy.Ascending)
        {
            IQueryable<T> query = _context.Set<T>();
            if (take.HasValue)
                query.Take(take.Value);
            if (skip.HasValue)
                query.Skip(skip.Value);
            if (orderBy != null)
            {
                if (orderByDirection == OrderBy.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }
            return await query.Where(match).ToListAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> match)
        {
            return await _context.Set<T>().AnyAsync(match);
        }

        public bool Any(Expression<Func<T, bool>> match)
        {
            return _context.Set<T>().Any(match);
        }
    }
}

