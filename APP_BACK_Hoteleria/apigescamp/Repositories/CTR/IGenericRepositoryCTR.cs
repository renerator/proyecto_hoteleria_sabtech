using DemoBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DemoBackend.Repositories
{
    public interface IGenericRepositoryCTR<T> 
    {
        Task<T> AddAsync(T entity);
        Task<T> FindAsync(object[] id);
        Task<T> InsertAsync(T entity);
        void Remove(T entity);
        Task<int> SaveChangesAsync();
        Task<IEnumerable<T>> WhereAsync(Expression<Func<T, bool>> match, string[] includes = null);
    }
}