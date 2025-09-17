using Microsoft.EntityFrameworkCore;
using DemoBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
namespace DemoBackend.RepositoryGes
{
    public sealed class GenericRepositoriesGes<T> : IDisposable, IGenericRepositoryEntity<T> where T : EntityBase
    {

        readonly string entityNula = "Entidad no puede ser nula";
        private readonly GESContext _dbContext;

        public GenericRepositoriesGes(GESContext dbContext) : base()
        {
            _dbContext = dbContext;
        }

        public void Delete(T entity)
        {

            if (entity == null)
            {
                throw new ArgumentNullException(entityNula);
            }

            _dbContext.Set<T>().Remove(entity);
            _dbContext.SaveChanges();

        }

        public void Dispose()
        {
            if (_dbContext != null)
            {
                _dbContext.Dispose();
            }
        }

        public List<T> FindAllWhere(Func<T, bool> criteria)
        {
            return _dbContext.Set<T>().Where(criteria).ToList();
        }

        public T Get(int id)
        {
            return _dbContext.Set<T>().Find(id);
        }

        public IEnumerable<T> GetAll()
        {
            return _dbContext.Set<T>().AsEnumerable();

        }

        public void Include<TProperty>(System.Linq.Expressions.Expression<Func<T, TProperty>> navigationPropertyPath)
        {
            _dbContext.Set<T>().Include(navigationPropertyPath);
        }

        public void Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(entityNula);
            }
            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();

        }

        public void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(entityNula);
            }

            _dbContext.Set<T>().Update(entity);
            _dbContext.SaveChanges();
        }

        public bool Exists(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().Any(predicate);
        }


        public IEnumerable<T> GetStoreProcedure(string query, params object[] parameters)
        {
            try
            {
                return _dbContext.Set<T>().FromSqlRaw(query, parameters).ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void InsertProcedure(string query, params object[] parameters)
        {
            try
            {
                _dbContext.Database.ExecuteSqlRaw(query, parameters);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void ExecuteProcedure(string query, params object[] parameters)
        {
            try
            {
                _dbContext.Database.ExecuteSqlRaw(query, parameters);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

