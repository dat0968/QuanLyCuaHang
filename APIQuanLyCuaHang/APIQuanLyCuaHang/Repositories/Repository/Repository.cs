using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using APIQuanLyCuaHang.Models;
using Microsoft.EntityFrameworkCore;

namespace APIQuanLyCuaHang.Repositories.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly QuanLyCuaHangContext _db;
        internal DbSet<T> dbSet;
        public Repository(QuanLyCuaHangContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();

        }
        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await dbSet.AddRangeAsync(entities);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> filter)
        {
            return await dbSet.AnyAsync(filter);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return await query.ToListAsync();
        }

        public async Task<T?> GetAsync(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<T> query;
            if (tracked)
            {
                query = dbSet;
            }
            else
            {
                query = dbSet.AsNoTracking();
            }

            query = query.Where(filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return await query.FirstOrDefaultAsync();
        }

        public void Remove(T entity)
        {
            _db.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _db.RemoveRange(entities);
        }
    }
}