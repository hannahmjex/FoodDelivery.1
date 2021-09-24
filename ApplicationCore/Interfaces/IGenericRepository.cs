using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        //Get object by key id
        T GetById(int id);

        //Get where a condition exists
        //Like a SELECT WHERE
        T Get(Expression<Func<T, bool>> predicate, bool asNoTracking = false, string includes = null);

        //same as above by async action
        Task<T> GetAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = false, string includes = null);

        //Returns an enumerable list of results
        IEnumerable<T> List();
        
        //returns enumerable list of results to iterate through
        IEnumerable<T> List(Expression<Func<T, bool>> predicate, Expression<Func<T, int>> orderBy = null, string includes = null);
        
        //same as above by async action
        Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, int>> orderBy = null, string includes = null);

        //Add a new record instance
        void Add(T entity);

        //delete a single record instance
        void Delete(T entity);

        //delete multiple record instances
        void Delete(IEnumerable<T> entities);

        //update all changes in an object
        void Update(T entity);
    }
}
