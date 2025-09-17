using DemoBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace DemoBackend.Repository
{
    public interface IGenericRepositoryCTREntity<T> where T : EntityBase
    {

        IEnumerable<T> GetAll();
        T Get(int id);
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
        List<T> FindAllWhere(Func<T, bool> criteria);
        void Include<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath);
        bool Exists(Expression<Func<T, bool>> predicate);

        IEnumerable<T> GetStoreProcedure(string query, params object[] parameters);

        void InsertProcedure( string query, params object[] parameters );
    }
}
