using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DemoBackend.Extensions;
using DemoBackend.Models;

namespace DemoBackend.Repositories
{
    public class GenericRepositoryCTR<T> : IGenericRepositoryCTR<T> where T : class
    {
        protected readonly CTRContext context;

        public GenericRepositoryCTR(CTRContext context)
        {
            this.context = context;
        }

        public async Task<T> AddAsync(T entity)
        {
            var result = await context.Set<T>().AddAsync(entity);

            return result.Entity;
        }

        public async Task<T> FindAsync(object[] id)
        {
            return await context.Set<T>().FindAsync(id);
        }

        public async Task<T> InsertAsync(T entity)
        {
            var result = await context.Set<T>().AddAsync(entity);

            return result.Entity;
        }

        public void Remove(T entity)
        {
            context.Remove(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> match, string[] includes = null)
        {
            var entity = context.Set<T>().AsQueryable();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    entity = entity.Include(include);
                }
            }

            return await entity.Where(match).ToListAsync();
        }
    }
}