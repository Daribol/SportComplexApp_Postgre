using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SportComplexApp.Data.Repository.Interfaces
{
    public interface IRepository<T>
    {
        T GetById(int id);

        Task<T> GetByIdAsync(int id);

        T FirstOrDefault(Func<T, bool> predicate);

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        IEnumerable<T> GetAll();

        Task<IEnumerable<T>> GetAllAsync();

        IQueryable<T> GetAllAttached();

        void Add(T item);

        Task AddAsync(T item);

        void AddRange(T[] items);

        Task AddRangeAsync(T[] items);

        bool Delete(T entity);

        Task<bool> DeleteAsync(T entity);

        bool Update(T item);

        Task<bool> UpdateAsync(T item);
    }
}
